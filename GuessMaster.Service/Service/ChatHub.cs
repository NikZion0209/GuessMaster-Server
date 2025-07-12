using GuessMaster.Model.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuessMaster.Model.Constants;

namespace GuessMaster.Service.Service
{
    public class ChatHub : Hub
    {
        public static event Action<int, string>? UserLeftRoom;
        public static event Action<int, List<ConnectedUser>>? UserJoinedRoom;

        private static readonly ConcurrentDictionary<int, List<ConnectedUser>> SessionUsers = new();

        // Method for joining a room
        public async Task JoinRoom(int sessionId, string userName, string avatarUrl)
        {
            Console.WriteLine($"{userName} is joining room {sessionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());
            await Clients.Group(sessionId.ToString()).SendAsync("RoomMessage", $"{userName} has joined the room");
            var user = new ConnectedUser
            {
                ConnectionId = Context.ConnectionId,
                Username = userName,
                AvatarUrl = avatarUrl
            };

            SessionUsers.AddOrUpdate(
                sessionId,
                _ => new List<ConnectedUser> { user },
                (_, list) =>
                {
                    lock (list)
                    {
                        if (!list.Any(u => u.ConnectionId == user.ConnectionId))
                            list.Add(user);
                    }
                    return list;
                }
            );

            await GetPlayersInSession(sessionId);

            if (SessionUsers.TryGetValue(sessionId, out var connectedUsers))
            {
                UserJoinedRoom?.Invoke(sessionId, connectedUsers);
            }
        }

        // Method for leaving a room
        public async Task LeaveRoom(int sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId.ToString());
            await Clients.Group(sessionId.ToString()).SendAsync("RoomUpdate", $"{Context.ConnectionId} left room {sessionId}");

            UserLeftRoom?.Invoke(sessionId, Context.ConnectionId);
        }

        // Method for broadcasting messages to the room
        public async Task SendRoomMessage(int sessionId, string userName, string message)
        {
            await Clients.Group(sessionId.ToString()).SendAsync("RoomMessage", $"{userName} : {message}");
        }

        public async Task GetPlayersInSession(int sessionId)
        {
            if (SessionUsers.TryGetValue(sessionId, out var users))
            {
                await Clients.Group(sessionId.ToString()).SendAsync("PlayersInSession", users);
            }
            else
            {
                Console.Error.WriteLine($"Error retrieving users for session {sessionId}");
            }
        }

        public async Task SendDrawing(int sessionId, string drawingData)
        {
            await Clients.Group(sessionId.ToString()).SendAsync("RecieveDrawing", drawingData);
        }
    }
}

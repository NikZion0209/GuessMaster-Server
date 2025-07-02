using GuessMaster.Model.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Service
{
    public class ChatHub : Hub
    {
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
        }

        // Method for leaving a room
        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("RoomUpdate", $"{Context.ConnectionId} left room {roomId}");
        }

        // Method for broadcasting messages to the room
        public async Task SendRoomMessage(int sessionId, string userName, string message)
        {
            await Clients.Group(sessionId.ToString()).SendAsync("RoomMessage", $"{userName} : {message}");
        }

        public async Task GetPlayersInSession(int sessionId)
        {
            Console.WriteLine("Hit");
            if (SessionUsers.TryGetValue(sessionId, out var users))
            {
                await Clients.Caller.SendAsync("PlayersInSession", users);
            }
            else
            {
                await Clients.Caller.SendAsync("PlayersInSession", new List<ConnectedUser>());
            }
        }
    }
}

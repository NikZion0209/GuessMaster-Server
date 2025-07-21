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
        public static event Action<int, int, string>? UserJoinedRoom;

        private static readonly ConcurrentDictionary<int, List<string>> SessionUsers = new();

        private void RemoveUserFromSession(int sessionId, string connectionId)
        {
            if (SessionUsers.TryGetValue(sessionId, out var users))
            {
                lock (users)
                {
                    var user = users.FirstOrDefault(connectionId);
                    if (user != null)
                    {
                        users.Remove(user);
                    }
                }
            }
        }

        public async Task JoinRoom(int sessionId, int userId)
        {
            Console.WriteLine($"{userId} is joining room {sessionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());

            SessionUsers.AddOrUpdate(
                sessionId,
                _ => new List<string> { Context.ConnectionId },
                (_, existingUsers) =>
                {
                    lock (existingUsers)
                    {
                        existingUsers.Add(Context.ConnectionId);
                    }
                    return existingUsers;
                }
            );

            UserJoinedRoom?.Invoke(sessionId, userId, Context.ConnectionId);
        }

        // Method for handling disconnection from a room
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var sessionId = SessionUsers
                .FirstOrDefault(kvp => kvp.Value.Any(ConnectionId => ConnectionId == Context.ConnectionId)).Key;

            RemoveUserFromSession(sessionId, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId.ToString());
            UserLeftRoom?.Invoke(sessionId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        // Method for leaving a room
        public async Task LeaveRoom(int sessionId)
        {
            RemoveUserFromSession(sessionId, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId.ToString());
            UserLeftRoom?.Invoke(sessionId, Context.ConnectionId);
        }

        // Method for broadcasting messages to the room
        public async Task SendRoomMessage(int sessionId, string userName, string message)
        {
            await Clients.Group(sessionId.ToString()).SendAsync(ChatEventNames.RoomMessage, $"{userName} : {message}");
        }

        public async Task SendDrawing(int sessionId, string drawingData)
        {
            await Clients.Group(sessionId.ToString()).SendAsync(ChatEventNames.RecieveDrawing, drawingData);
        }
    }
}

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

        public static readonly ConcurrentDictionary<int, List<string>> SessionUsers = new();

        private void RemoveUserFromSession(int sessionId, string connectionId)
        {
            if (SessionUsers.TryGetValue(sessionId, out var users))
            {
                lock (users)
                {
                    var user = users.FirstOrDefault(user => user == connectionId);
                    if (user != null)
                    {
                        users.Remove(user);
                    }
                }
            }
        }

        public async Task JoinRoom(int sessionId, int userId)
        {
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
            var sessionEntry = SessionUsers
                .FirstOrDefault(kvp => kvp.Value.Any(ConnectionId => ConnectionId == Context.ConnectionId));

            // If no session found, skip further processing
            if (sessionEntry.Value == null)
            {
                Console.WriteLine($"User with connection ID {Context.ConnectionId} disconnected but no session found.");
                await base.OnDisconnectedAsync(exception);
                return;
            }

            var sessionId = sessionEntry.Key;

            Console.WriteLine($"User with connection ID {Context.ConnectionId} disconnected from session {sessionId} - chat hub.");
            RemoveUserFromSession(sessionId, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId.ToString());
            UserLeftRoom?.Invoke(sessionId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        // Method for leaving a room
        public async Task LeaveRoom(int sessionId)
        {
            Console.WriteLine($"User with connection ID {Context.ConnectionId} voluntarily disconnected from the chat hub.");
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
            try
            {
                await Clients.Group(sessionId.ToString()).SendAsync(ChatEventNames.RecieveDrawing, drawingData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending drawing data: {ex.Message}");

            }
        }
    }
}

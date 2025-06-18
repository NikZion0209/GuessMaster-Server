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
        // Method for joining a room
        public async Task JoinRoom(int sessionId)
        {
            Console.WriteLine($"{Context.ConnectionId} is joining room {sessionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());
            await Clients.Group(sessionId.ToString()).SendAsync("RoomUpdate", $"{Context.ConnectionId} joined room {sessionId}");
        }

        // Method for leaving a room
        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("RoomUpdate", $"{Context.ConnectionId} left room {roomId}");
        }

        // Method for broadcasting messages to the room
        public async Task SendRoomMessage(int sessionId, string message)
        {
            await Clients.Group(sessionId.ToString()).SendAsync("RoomMessage", message);
        }
    }
}

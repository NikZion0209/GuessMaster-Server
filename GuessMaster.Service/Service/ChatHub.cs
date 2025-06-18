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
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("RoomUpdate", $"{Context.ConnectionId} joined room {roomId}");
        }

        // Method for leaving a room
        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("RoomUpdate", $"{Context.ConnectionId} left room {roomId}");
        }

        // Method for broadcasting messages to the room
        public async Task SendRoomMessage(string roomId, string message)
        {
            await Clients.Group(roomId).SendAsync("RoomMessage", message);
        }
    }
}

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
        private static readonly ConcurrentDictionary<string, HashSet<string>> MuteList =
      new ConcurrentDictionary<string, HashSet<string>>();
        //// Join a specific chat room
        //public async Task JoinRoom(string roomName)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        //    await Clients.Group(roomName).SendAsync("ReceiveMessage", new
        //    {
        //        UserName = "System",
        //        Message = $"{Context.ConnectionId} has joined the room {roomName}",
        //        Timestamp = DateTime.UtcNow
        //    });
        //}

        //// Leave a specific chat room
        //public async Task LeaveRoom(string roomName)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        //    await Clients.Group(roomName).SendAsync("ReceiveMessage", new
        //    {
        //        UserName = "System",
        //        Message = $"{Context.ConnectionId} has left the room {roomName}",
        //        Timestamp = DateTime.UtcNow
        //    });
        //}

        //// Send a message to a specific room
        //public async Task SendMessageToRoom(string roomName, string userName, string message)
        //{
        //    var chatMessage = new
        //    {
        //        UserName = userName,
        //        Message = message,
        //        Timestamp = DateTime.UtcNow
        //    };
        //    await Clients.Group(roomName).SendAsync("ReceiveMessage", chatMessage);
        //}
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
            if (MuteList.ContainsKey(roomId) && MuteList[roomId].Contains(Context.ConnectionId))
            {
            }
            else
            {
                await Clients.Group(roomId).SendAsync("RoomMessage", message);
            }

        }
        public async Task MuteUser(string roomId, string targetConnectionId)
        {
            if (!MuteList.ContainsKey(roomId))
            {
                MuteList[roomId] = new HashSet<string>();
            }

            MuteList[roomId].Add(targetConnectionId);
            await Clients.Client(targetConnectionId).SendAsync("Muted", $"You have been muted in room {roomId}");
        }

        // Unmute a user
        public async Task UnmuteUser(string roomId, string targetConnectionId)
        {
            if (MuteList.ContainsKey(roomId))
            {
                MuteList[roomId].Remove(targetConnectionId);
                await Clients.Client(targetConnectionId).SendAsync("Unmuted", $"You have been unmuted in room {roomId}");
            }
        }

    }
}

using GuessMaster.Data.Models;
using GuessMaster.Model.ViewModel;
using GuessMaster.Repository;
using GuessMaster.Service.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GuessMaster.Service.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static List<WebSocket> clients = new List<WebSocket>();


        public RoomService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor)
        {
            _repositoryManager = repositoryManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<Room> GetAllRoomList()
        {
            try
            {
                return _repositoryManager.RoomRepository.GetAllRoomList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task GetLobbyDetailsViaWebSocketAsync(int roomId)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.WebSockets.IsWebSocketRequest == true) // Check if the request is a WebSocket request
            {
                var roomDetails = await _repositoryManager.RoomRepository.GetRoomDetailsByRoomIdAsync(roomId);
                WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                clients.Add(webSocket); // Add the WebSocket client to the list
                await HandleClient(webSocket, roomDetails); // Handle communication with the WebSocket client

            }
            //else
            //{
            //    httpContext.Response.StatusCode = 400;
            //}
            //if (httpContext == null)
            //{
            //    Console.WriteLine("HttpContext is null.");
            //    return;
            //}

            //if (httpContext.WebSockets.IsWebSocketRequest)
            //{
            //    WebSocketsService ws = new WebSocketsService();
            //    var socket = await httpContext.WebSockets.AcceptWebSocketAsync();
            //    var clientId = Guid.NewGuid().ToString();

            //    ws.AddSocket(clientId, socket);

            //    try
            //    {
            //        while (socket.State == WebSocketState.Open)
            //        {
            //            // Fetch Room Details
            //            var roomDetails = await _repositoryManager.RoomRepository.GetRoomDetailsByRoomIdAsync(roomId);

            //            // Broadcast Room Details to All Connected Clients
            //            await ws.BroadcastAsync(roomDetails);

            //            // Delay before next update
            //            await Task.Delay(1000);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error in WebSocket connection: {ex.Message}");
            //    }
            //    finally
            //    {
            //        ws.RemoveSocket(clientId);
            //        if (socket.State == WebSocketState.Open)
            //        {
            //            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
            //        }
            //    }
            //}
            //else
            //{
            //    httpContext.Response.StatusCode = 400;
            //}
        }


        public Room GetRoomById(int id)
        {
            try
            {
                return _repositoryManager.RoomRepository.GetRoomById(id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<RoomDetail> GetRoomDetailsByRoomIdAsync(int roomId)
        {
            try
            {
                var room = await _repositoryManager.RoomRepository.GetRoomDetailsByRoomIdAsync(roomId);
                RoomDetail roomDetails = new RoomDetail();
                if (room != null)
                {

                    roomDetails.RoomId = room.RoomId;
                    roomDetails.IsFull = Convert.ToBoolean(room.IsFull);
                    roomDetails.PlayerCount = int.Parse(room.PlayerCount.ToString());
                    roomDetails.CreatedAt = Convert.ToDateTime(room.CreatedAt);
                    roomDetails.Users = room.RoomAssignments.Select(ra => new RoomUser
                    {
                        UserId = ra.User.UserId,
                        Username = ra.User.Username,
                        IsGuest = ra.User.IsGuest
                    }).ToList();
                    return roomDetails;
                }
                else
                {
                    return roomDetails;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Room> JoinOrCreateRoomAsync(int UserId)
        {
            try
            {
                var availableRoom = await _repositoryManager.RoomRepository.AvailableRoomAsync();
                if (availableRoom == null)
                {
                    availableRoom = new Room();
                    await _repositoryManager.RoomRepository.SaveRoom(availableRoom);
                }
                var assignment = new RoomAssignment
                {
                    RoomId = availableRoom.RoomId,
                    UserId = UserId,
                    AssignedAt = DateTime.Now
                };
                availableRoom.PlayerCount++;
                if (availableRoom.PlayerCount == 8)
                {
                    availableRoom.IsFull = true;
                }
                _repositoryManager.RoomAssignmentRepository.SaveRoomAssignment(assignment);
                return availableRoom;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool RemoveRoom(Room room)
        {
            try
            {
                return _repositoryManager.RoomRepository.RemoveRoom(room);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Room> SaveRoom(Room room)
        {
            try
            {
                return await _repositoryManager.RoomRepository.SaveRoom(room);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task HandleClient(WebSocket clientSocket, Room roomDetails)
        {
            var buffer = new byte[1024 * 4];

            while (clientSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var receivedObject = JsonSerializer.Deserialize<Room>(receivedMessage);
                        var responseObject = new Room
                        {
                            RoomId = roomDetails.RoomId
                        };

                        string responseJson = JsonSerializer.Serialize(responseObject);

                        // Broadcast the JSON object to all connected clients
                        foreach (var client in clients)
                        {
                            if (client.State == WebSocketState.Open)
                            {
                                await client.SendAsync(
                                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(responseJson)),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None
                                );
                            }
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        clients.Remove(clientSocket);
                        Console.WriteLine("Client disconnected. Total clients: " + clients.Count);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    break;
                }
            }
        }

        public async Task<Room> GetLobbyDetailsViaw(int roomId)
        {
            try
            {
                return await _repositoryManager.RoomRepository.GetRoomDetailsByRoomIdAsync(roomId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Room> GetLobbyDetailsByUserName(string UserName)
        {
            try
            {
               return await _repositoryManager.RoomRepository.GetRoomDetailsByUserNameAsync(UserName);  
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

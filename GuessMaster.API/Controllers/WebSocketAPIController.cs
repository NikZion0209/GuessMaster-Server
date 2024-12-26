using GuessMaster.Data.Data;
using GuessMaster.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;

namespace GuessMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebSocketAPIController : ControllerBase
    {
        private static List<WebSocket> clients = new List<WebSocket>();
        private readonly IRepositoryManager _repositoryManager;
        // Inject the ApplicationDbContext
        public WebSocketAPIController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        [HttpGet("connect")]
        public async Task Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                clients.Add(webSocket);
                await HandleClient(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400; // Bad Request
            }
        }

        private async Task HandleClient(WebSocket clientSocket)
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
                        Console.WriteLine("Message received: " + receivedMessage);

                        string username = receivedMessage;
                        var user = await _repositoryManager.RoomRepository.GetRoomDetailsByUserNameAsync(username);
                        if (user != null)
                        {
                            // Prepare the data to send
                            string messageToSend = $"{username},{user.RoomAssignments.FirstOrDefault().UserId}";

                            // Broadcast the message to all connected clients
                            foreach (var client in clients)
                            {
                                if (client.State == WebSocketState.Open)
                                {
                                    if (client != clientSocket && client.State == WebSocketState.Open)
                                    {
                                        await client.SendAsync(
                                        new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend)),
                                        WebSocketMessageType.Text,
                                        true,
                                        CancellationToken.None
                                    );
                                    }
                                }
                            }
                        }
                        else
                        {
                            // If user is not found, send a message indicating that
                            string messageToSend = $"User {username} not found in the database.";
                            foreach (var client in clients)
                            {
                                if (client.State == WebSocketState.Open)
                                {
                                    await client.SendAsync(
                                        new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend)),
                                        WebSocketMessageType.Text,
                                        true,
                                        CancellationToken.None
                                    );
                                }
                            }
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        clients.Remove(clientSocket);
                        await clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    break;
                }
            }
        }
    }
}

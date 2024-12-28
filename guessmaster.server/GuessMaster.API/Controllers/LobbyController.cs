using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("ws")]
public class LobbyController : ControllerBase
{
    [HttpGet("lobby")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await SendUpdates(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }

    private async Task SendUpdates(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var players = new[] { "Player1", "Player2", "Player3" }; // Replace with actual logic
        while (webSocket.State == WebSocketState.Open)
        {
            foreach (var player in players)
            {
                var data = Encoding.UTF8.GetBytes(player);
                await webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            await Task.Delay(5000); // Send updates every 5 seconds
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Interface
{
    public interface IWebSocketsService
    {
        public void AddSocket(string id, WebSocket socket);
        public void RemoveSocket(string id);
        public Task BroadcastAsync(object data);
    }
}

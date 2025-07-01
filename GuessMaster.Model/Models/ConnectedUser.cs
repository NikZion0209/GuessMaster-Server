using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Model.Models
{
    public class ConnectedUser
    {
        public required string ConnectionId { get; set; }
        public required string Username { get; set; }
        public required string AvatarUrl { get; set; }
        public string GuessMasterDrawing { get; set; } = string.Empty;
    }
}

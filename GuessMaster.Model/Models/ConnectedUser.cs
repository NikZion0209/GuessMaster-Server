using GuessMaster.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Model.Models
{
    public class ConnectedUser : User
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string GuessMasterDrawing { get; set; } = string.Empty;
    }
}

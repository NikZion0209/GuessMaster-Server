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
        public int Ratings { get; set; } = 0;
        public int Score { get; set; } = 0;
        public string Drawing { get; set; } = string.Empty;
    }
}

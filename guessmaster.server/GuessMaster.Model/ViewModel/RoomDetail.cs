using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Model.ViewModel
{
    public class RoomDetail
    {
        public int RoomId { get; set; }
        public bool IsFull { get; set; }
        public int PlayerCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<RoomUser> Users { get; set; } = new List<RoomUser>();
    }

    public class RoomUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public bool IsGuest { get; set; }
    }
}

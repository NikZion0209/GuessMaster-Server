using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Model.Models
{
    public class Leaderboards
    {
        public int LeaderBoardRecordId { get; set; }
        public required int Gamemode { get; set; }
        public required string AvatarId { get; set; }
        public required string Username { get; set; }
        public required int Score { get; set; }
    }
}

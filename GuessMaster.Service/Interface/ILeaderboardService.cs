using GuessMaster.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Interface
{
    public interface ILeaderboardService
    {
        List<SessionUserDto> GetTopTenPlayers(int gameType);
        void GetPlayerRank(int gameType, string username, out int rank, out int score);
    }
}

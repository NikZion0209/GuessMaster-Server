using GuessMaster.Model.Models;
using GuessMaster.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Interface
{
    public interface ILeaderboardRepository
    {
        List<SessionUserDto> GetTopTenPlayers(int gameType);
        void AddLeaderboardEntry(Leaderboards entry);
        void GetPlayerRank(int gameType, string username, out int rank, out int score);
        void ClearLeaderboard(int gameType);
        void ClearAllLeaderboards();
    }
}

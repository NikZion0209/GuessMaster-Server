using GuessMaster.Model.ViewModel;
using GuessMaster.Repository;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Service
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly IRepositoryManager _repositoryManager;
        public LeaderboardService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public List<SessionUserDto> GetTopTenPlayers(int gameType)
        {
            return _repositoryManager.LeaderboardRepository.GetTopTenPlayers(gameType);
        }
    }
}

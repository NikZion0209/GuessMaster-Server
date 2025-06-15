using GuessMaster.Data.Models;
using GuessMaster.Repository;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Service
{
    public class GameSessions : IGameSessions
    {
        private readonly IRepositoryManager _repositoryManager;

        public GameSessions(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public List<GameSession> GetAvailableGameSessions(int gameType)
        {
            try
            {
                List<GameSession> sessions = _repositoryManager.GameSessionRepository.GetAvailableGameSessions(gameType);
                return sessions;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while retrieving game sessions.", ex);
            }
        }
    }
}

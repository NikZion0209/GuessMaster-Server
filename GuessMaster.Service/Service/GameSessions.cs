using GuessMaster.Data.Models;
using GuessMaster.Model.Constants;
using GuessMaster.Model.Models;
using GuessMaster.Repository;
using GuessMaster.Repository.Interface;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Service
{
    public class GameSessions : IGameSessions
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IDoodleChampRepository _doodleChampRepository;

        public GameSessions(IRepositoryManager repositoryManager, IDoodleChampRepository doodleChampRepository)
        {
            _repositoryManager = repositoryManager;
            _doodleChampRepository = doodleChampRepository;
        }

        public class GameSessionDTO
        {
            public int SessionId { get; set; }
            public int PlayerCount { get; set; }
            public int MaxPlayers { get; set; }
        }

        public List<GameSessionDTO> GetAvailableGameSessions(int gameType)
        {
            try
            {
                switch (gameType)
                {
                    case Gamemodes.DoodleChamp:
                        _doodleChampRepository.GetAvailableSessions(out var availableSessions);

                        if (availableSessions == null || !availableSessions.Any())
                        {
                            _doodleChampRepository.CreateNewSession(out availableSessions);
                        }

                        return availableSessions.Select(s => new GameSessionDTO
                        {
                            SessionId = s.SessionId,
                            PlayerCount = s.PlayerCount,
                            MaxPlayers = s.MaxPlayers
                        }).ToList();

                    default:
                        throw new ArgumentException("Invalid game type specified.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while retrieving game sessions.", ex);
            }
        }

        public void AddUserToSession(int gameType, int sessionId, int userId)
        {
            try
            {
                User user = _repositoryManager.PlayerRepository.GetUserById(userId);
                switch (gameType)
                {
                    case Gamemodes.DoodleChamp:
                        _doodleChampRepository.AddUserToSession(sessionId, user);
                        break;
                    default:
                        throw new ArgumentException("Invalid game type specified.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                Console.WriteLine($"Error adding user to session: {ex.Message}");
                throw new Exception("An error occurred while adding user to session.", ex);
            }
        }

        public int AddUserToNextAvailableSession(int gameType, int userId, out int sessionId)
        {
            try
            {
                User user = _repositoryManager.PlayerRepository.GetUserById(userId);
                switch (gameType)
                {
                    case Gamemodes.DoodleChamp:
                        _doodleChampRepository.AddUserToNextAvailableSession(user, out int doodleChampSessionId);
                        sessionId = doodleChampSessionId;
                        break;
                    default:
                        throw new ArgumentException("Invalid game type specified.");
                }
                return sessionId;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                Console.WriteLine($"Error adding user to session: {ex.Message}");
                throw new Exception("An error occurred while adding user to session.", ex);
            }
        }
    }
}

using GuessMaster.Data.Data;
using GuessMaster.Data.Models;
using GuessMaster.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Repository
{
    public class GameSession : IGameSession
    {
        private readonly ApplicationDbContext _context;

        public GameSession(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Data.Models.GameSession> GetAvailableGameSessions(int gameType)
        {
            try
            {
                return _context.GameSessions
                    .Where(gs => !gs.IsFull && !gs.InPlay && gs.GameType == gameType)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving game sessions.", ex);
            }
        }

        public List<Data.Models.GameSession> CreateNewSession(int gameType)
        {
            try
            {
                var newSession = new Data.Models.GameSession
                {
                    GameType = gameType,
                    CreatedAt = DateTime.Now
                };
                _context.GameSessions.Add(newSession);
                _context.SaveChanges();
                return new List<Data.Models.GameSession> { newSession };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating a new game session.", ex);
            }
        }

        public Data.Models.GameSession? GetSessionById(int sessionId)
        {
            try
            {
                var session = _context.GameSessions.Find(sessionId);
                if (session == null)
                {
                    throw new Exception($"Game session with ID {sessionId} not found.");
                }
                return session;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving game session by ID {sessionId}: {ex.Message}");
                throw new Exception("An error occurred while retrieving the game session by ID.", ex);
            }
        }

        public void UpdateSession(Data.Models.GameSession session)
        {
            try
            {
                _context.GameSessions.Update(session);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the game session.", ex);
            }
        }
    }
}

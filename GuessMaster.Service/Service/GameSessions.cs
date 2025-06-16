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
                if (sessions == null || !sessions.Any())
                {
                    sessions = _repositoryManager.GameSessionRepository.CreateNewSession(gameType);
                }
                return sessions;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while retrieving game sessions.", ex);
            }
        }

        public void AddUserToSession(int sessionId, int userId)
        {
            try
            {
                var session = _repositoryManager.GameSessionRepository.GetSessionById(sessionId);
                if (session == null)
                {
                    throw new Exception("Session not found.");
                }
                if (session.IsFull)
                {
                    throw new Exception("Session is full.");
                }
                Console.WriteLine($"Adding user with ID {userId} to session with ID {sessionId}.");
                User joiningUser = _repositoryManager.PlayerRepository.GetUserById(userId);
                if (joiningUser == null)
                {
                    throw new Exception("User not found.");
                }

                session.Users.Add(joiningUser);
                session.PlayerCount++;
                if (session.PlayerCount >= session.MaxPlayers)
                {
                    session.IsFull = true;
                }
                Console.WriteLine($"User {joiningUser.Username} added to session {sessionId}.");
                _repositoryManager.GameSessionRepository.UpdateSession(session);
                Console.WriteLine($"Session {sessionId} updated successfully.");
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

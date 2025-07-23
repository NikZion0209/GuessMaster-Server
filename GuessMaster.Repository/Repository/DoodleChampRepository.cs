using GuessMaster.Data.Models;
using GuessMaster.Model.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuessMaster.Repository.Interface;

namespace GuessMaster.Repository.Repository
{
    public class DoodleChampRepository : IDoodleChampRepository
    {
        private readonly ConcurrentDictionary<int, DoodleChampSession> Sessions = new();

        public bool TryGetSession(int sessionId, out DoodleChampSession? session) => Sessions.TryGetValue(sessionId, out session);

        public void CreateNewSession(out List<DoodleChampSession> doodleChampSession)
        {
            int sessionId = Sessions.Count > 0 ? Sessions.Keys.Max() + 1 : 1; // Generate a new session ID

            var newSession = new DoodleChampSession
            {
                SessionId = sessionId
            };
            if (Sessions.TryAdd(sessionId, newSession))
            {
                GetAvailableSessions(out doodleChampSession);
                Console.WriteLine($"New DoodleChamp session created with ID {sessionId}.");
            }
            else
            {
                throw new InvalidOperationException($"Failed to create a new session with ID {sessionId}.");
            }
        }

        public void RemoveSession(int sessionId)
        {
            if (!Sessions.TryRemove(sessionId, out _))
            {
                throw new InvalidOperationException($"Session {sessionId} does not exist.");
            }
        }

        public void RemoveUserFromSession(int sessionId, string connectionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                var user = session.ConnectedUsers.FirstOrDefault(u => u.ConnectionId == connectionId);
                if (user != null)
                {
                    session.ConnectedUsers.Remove(user);
                    session.PlayerCount = session.ConnectedUsers.Count;
                    Console.WriteLine($"User {user.Username} removed from session {sessionId}.");
                }
                else
                {
                    throw new KeyNotFoundException($"User with connection ID {connectionId} not found in session {sessionId}.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void GetSessionUsers(int sessionId, out List<ConnectedUser> users)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                users = session.ConnectedUsers;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void GetSessionState(int sessionId, out int sessionState)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                sessionState = session.GameState;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void UpdateSessionUsers(int sessionId, List<ConnectedUser> users)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.ConnectedUsers = users;
                session.PlayerCount = users.Count;
            }
            else
            {
                throw new InvalidOperationException($"Session {sessionId} does not exist.");
            }
        }

        public void UpdateSessionState(int sessionId, int state)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.GameState = state;
            }
            else
            {
                throw new InvalidOperationException($"Session {sessionId} does not exist.");
            }
        }

        public bool CheckIfSessionExists(int sessionId)
        {
            return Sessions.ContainsKey(sessionId);
        }

        public void GetSessionPrompt(int sessionId, out string prompt)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                prompt = session.SelectedPrompt;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void SetSessionPrompt(int sessionId, string prompt)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.SelectedPrompt = prompt;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void GetSessionUsersTurn(int sessionId, out string connectionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                connectionId = session.UsersTurn;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void ResetSessionUsersTurn(int sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.UsersTurn = string.Empty;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void GetAvailableSessions(out List<DoodleChampSession> availableSessions)
        {
            availableSessions = Sessions.Values
                .Where(s => !s.IsFull && (s.GameState == Model.Constants.DoodleChamp.PreGame || s.GameState == Model.Constants.DoodleChamp.Lobby))
                .ToList();
        }

        public void AddUserToSession(int sessionId, User user)
        {
            ConnectedUser connectedUser = new ConnectedUser
            {
                UserId = user.UserId,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl
            };

            if (Sessions.TryGetValue(sessionId, out var session))
            {
                if (!session.IsFull && (session.GameState == Model.Constants.DoodleChamp.PreGame || session.GameState == Model.Constants.DoodleChamp.Lobby))
                {
                    session.ConnectedUsers.Add(connectedUser);
                    session.PlayerCount = session.ConnectedUsers.Count;
                    Console.WriteLine($"User {connectedUser.Username} added to session {sessionId}.");
                }
                else
                {
                    throw new InvalidOperationException($"Session {sessionId} is full or in play.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void UpdateUserConnectionId(int sessionId, int userId, string connectionId)
        {
            GetSessionUsers(sessionId, out List<ConnectedUser> users);
            var user = users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.ConnectionId = connectionId;
            }
            else
            {
                throw new KeyNotFoundException($"User with ID {userId} not found in session {sessionId}.");
            }
        }

        public string GetUserNameByUserId(int sessionId, int userId)
        {
            GetSessionUsers(sessionId, out List<ConnectedUser> users);
            var user = users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                return user.Username;
            }
            else
            {
                throw new KeyNotFoundException($"User with user ID {userId} not found in session {sessionId}.");
            }
        }

        public string GetUserNameByConnectionId(int sessionId, string connectionId)
        {
            GetSessionUsers(sessionId, out List<ConnectedUser> users);
            var user = users.FirstOrDefault(u => u.ConnectionId == connectionId);
            if (user != null)
            {
                return user.Username;
            }
            else
            {
                throw new KeyNotFoundException($"User with connection ID {connectionId} not found in session {sessionId}.");
            }
        }

        public void GenerateOrderOfPlay(int sessionId)
        {
            GetSessionUsers(sessionId, out var users);

            var random = new Random();
            var orderedUsers = users.OrderBy(x => random.Next()).ToList();

            UpdateSessionUsers(sessionId, orderedUsers);
            Console.WriteLine($"Order of play generated for session {sessionId}.");
        }

        public void UpdatePlayerTurn(int sessionId, string username)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.UsersTurn = username;

                Console.WriteLine($"{username} turn in session {sessionId}.");
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }
    }
}

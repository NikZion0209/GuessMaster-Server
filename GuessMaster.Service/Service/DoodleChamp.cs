using GuessMaster.Data.Models;
using GuessMaster.Model.Constants;
using GuessMaster.Model.Models;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GuessMaster.Service.Service
{
    public class DoodleChamp : IDoodleChamp
    {
        private readonly IGameTimer _gameTimer;

        private static readonly ConcurrentDictionary<int, DoodleChampSession> Sessions = new();

        public static event Action<int>? StartingLobbyTimer;
        public static event Action<int>? StoppingLobbyTimer;
        public static event Action<int>? GameStarted;
        public static event Action<int>? GameRestart;
        public static event Action<int>? GameEndedEarly;
        public static event Action<int, List<User>>? UpdatePlayerLeaderboard;

        public DoodleChamp(IGameTimer gameTimer)
        {
            _gameTimer = gameTimer;
        }

        public void RemoveSession(int sessionId)
        {
            if (!Sessions.TryRemove(sessionId, out _))
            {
                throw new InvalidOperationException($"Session {sessionId} does not exist.");
            }
        }

        private void RemoveUserFromSession(int sessionId, string connectionId)
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

        private void GetSessionState(int sessionId, out int sessionState)
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

        private void UpdateSessionUsers(int sessionId, List<ConnectedUser> users)
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

        private void UpdateSessionState(int sessionId, int state)
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

        private void GetSessionUsersTurn(int sessionId, out string connectionId)
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

        public void GetAvailableSessions(out List<DoodleChampSession> availableSessions)
        {
            availableSessions = Sessions.Values
                .Where(s => !s.IsFull && (s.GameState == Model.Constants.DoodleChamp.PreGame || s.GameState == Model.Constants.DoodleChamp.Lobby))
                .ToList();
        }

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
            if (user != null) { 
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

        public void CheckLobbyStatus(int sessionId)
        {
            Sessions.TryGetValue(sessionId, out var session);
            if (session == null)
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }

            if (session.ConnectedUsers.Count >= Model.Constants.DoodleChamp.MinPlayers)
            {
                switch (session.GameState)
                {
                    case Model.Constants.DoodleChamp.PreGame:
                        Console.WriteLine($"Session {sessionId} has enough players to start the game.");
                        StartGame(sessionId);
                        break;
                    case Model.Constants.DoodleChamp.Lobby:
                        Console.WriteLine($"Session {sessionId} is already in the lobby state.");
                        StartingLobbyTimer?.Invoke(sessionId);
                        GenerateOrderOfPlay(sessionId);
                        break;
                    default:
                        return;
                }
                
            }

        }

        public async Task StartGame(int sessionId)
        {
            Console.WriteLine($"Starting Lobby timer for session {sessionId}");
            UpdateSessionState(sessionId, Model.Constants.DoodleChamp.Lobby);
            StartingLobbyTimer?.Invoke(sessionId);

            await _gameTimer.StartTimer(
                sessionId, 
                Model.Constants.DoodleChamp.LobbyTimer, 
                Model.Constants.DoodleChamp.LobbyCountdown, 
                Gamemodes.DoodleChamp,
                Model.Constants.DoodleChamp.OrderOfPlayCountdown,
                Model.Constants.DoodleChamp.OrderOfPlayList
            );

            UpdateSessionState(sessionId, Model.Constants.DoodleChamp.InGame);
            GameStarted?.Invoke(sessionId);

            await _gameTimer.StartTimer(
                sessionId,
                Model.Constants.DoodleChamp.DrawingTimer,
                Model.Constants.DoodleChamp.DrawingCountdown,
                Gamemodes.DoodleChamp
            );
        }

        public void RemoveFromSession(int sessionId, string connectionId)
        {
            RemoveUserFromSession(sessionId, connectionId);

            GetSessionUsers(sessionId, out List<ConnectedUser> currentUsers);
            GetSessionState(sessionId, out int gameState);

            if (
                currentUsers.Count < Model.Constants.DoodleChamp.MinPlayers && 
                (gameState == Model.Constants.DoodleChamp.Lobby || gameState == Model.Constants.DoodleChamp.InGame)
            )
            {
                Console.WriteLine($"Not enough players in session {sessionId}.");
                _gameTimer.PauseTimer(sessionId);

                switch (gameState)
                {
                    case Model.Constants.DoodleChamp.Lobby:
                        Console.WriteLine($"Lobby timer paused for session {sessionId} due to insufficient players.");
                        int time = _gameTimer.GetTimerLength(sessionId);
                        
                        if (time < Model.Constants.DoodleChamp.QuickLobbyCountdown)
                        {
                            _gameTimer.SetTimerLength(sessionId, Model.Constants.DoodleChamp.QuickLobbyCountdown);
                        }
                        GameRestart?.Invoke(sessionId);
                        UpdateSessionState(sessionId, Model.Constants.DoodleChamp.PreGame);
                        break;
                    case Model.Constants.DoodleChamp.InGame:
                        Console.WriteLine($"Game ended for session {sessionId} due to insufficient players.");
                        _gameTimer.CancelTimer(sessionId);
                        GameEndedEarly?.Invoke(sessionId);
                        break;
                    default:
                        Console.WriteLine($"Unknown game state for session {sessionId}.");
                        break;
                }
                return;
            }

            var formattedUsers = currentUsers.Select(user => new User
            {
                Username = user.Username,
                AvatarUrl = user.AvatarUrl
            }).ToList();
            UpdatePlayerLeaderboard?.Invoke(sessionId, formattedUsers);

            GetSessionUsersTurn(sessionId, out string currentTurnConnectionId);

            if (gameState == Model.Constants.DoodleChamp.InGame && connectionId == currentTurnConnectionId)
            {
                _gameTimer.CancelTimer(sessionId);
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
    }
}

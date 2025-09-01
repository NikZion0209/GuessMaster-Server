using GuessMaster.Data.Models;
using GuessMaster.Model.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuessMaster.Repository.Interface;
using GuessMaster.Model.ViewModel;

namespace GuessMaster.Repository.Repository
{
    public class DoodleChampRepository : IDoodleChampRepository
    {
        private readonly ConcurrentDictionary<int, DoodleChampSession> Sessions = new();

        public static event Action<int, List<SessionUserDto>>? UpdatePlayerLeaderboard;

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
            Console.WriteLine($"Session {sessionId} removed successfully.");
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

        public void GetSessionUsersTurn(int sessionId, out string username)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                username = session.UsersTurn;
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

        public void SetSessionUsersTurn(int sessionId, string username)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.UsersTurn = username;
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

        public int AddUserToNextAvailableSession(User user, out int sessionId)
        {
            GetAvailableSessions(out var availableSessions);
            if (availableSessions.Count == 0)
            {
                CreateNewSession(out availableSessions);
            }
            sessionId = availableSessions[0].SessionId;
            AddUserToSession(sessionId, user);
            return sessionId;
        }

        public void AddUserToSession(int sessionId, User user)
        {
            ConnectedUser connectedUser = new()
            {
                UserId = user.UserId,
                Username = user.Username,
                AvatarId = user.AvatarId,
                Password = user.Password,
                Email = user.Email
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

        public void GetGuessCount(int sessionId, out int guessCount)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                guessCount = session.GuessedCorrectly;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void IncrementGuessCount(int sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.GuessedCorrectly++;
                Console.WriteLine($"Guess count incremented for session {sessionId}. Current count: {session.GuessedCorrectly}");
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void ResetGuessCount(int sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.GuessedCorrectly = 0;
                Console.WriteLine($"Guess count reset for session {sessionId}.");
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void GetPlayerCount(int sessionId, out int playerCount)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                playerCount = session.PlayerCount;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void GetCorrectUsers(int sessionId, out List<String> users)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                users = session.GuessedUsers;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void AddCorrectUser(int sessionId, string username)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                if (!session.GuessedUsers.Contains(username))
                {
                    session.GuessedUsers.Add(username);
                    Console.WriteLine($"{username} added to correct users in session {sessionId}.");
                }
                else
                {
                    Console.WriteLine($"{username} is already in the correct users list for session {sessionId}.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void ResetCorrectUsers(int sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.GuessedUsers.Clear();
                Console.WriteLine($"Correct users list reset for session {sessionId}.");
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void GetConnectionIdByUsername(int sessionId, string username, out string connectionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                var user = session.ConnectedUsers.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    connectionId = user.ConnectionId;
                }
                else
                {
                    throw new KeyNotFoundException($"User with username {username} not found in session {sessionId}.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void ResetReleasedHints(int sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.ReleasedHintPositions.Clear();
                Console.WriteLine($"Released hints reset for session {sessionId}.");
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void AddReleasedHintPosition(int sessionId, int position)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                if (!session.ReleasedHintPositions.Contains(position))
                {
                    session.ReleasedHintPositions.Add(position);
                    Console.WriteLine($"Released hint position {position} added for session {sessionId}.");
                }
                else
                {
                    Console.WriteLine($"Released hint position {position} already exists for session {sessionId}.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void GetReleasedHintPositions(int sessionId, out List<int> positions)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                positions = session.ReleasedHintPositions;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void ResetUserScores(int sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                foreach (var user in session.ConnectedUsers)
                {
                    user.Score = 0; // Reset each user's score to 0
                }
                Console.WriteLine($"User scores reset for session {sessionId}.");
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void ResetUsersRatings(int sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                foreach (var user in session.ConnectedUsers)
                {
                    user.Ratings = 0;
                }
                Console.WriteLine($"User ratings reset for session {sessionId}.");
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void IncrementUserRating(int sessionId, string username)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                var user = session.ConnectedUsers.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    user.Ratings++;
                    Console.WriteLine($"User {user.Username}'s rating incremented in session {sessionId}.");
                }
                else
                {
                    throw new KeyNotFoundException($"User with username {username} not found in session {sessionId}.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void GetSessionHighestRating(int sessionId, out string highestRatingUsername, out int highestRating, out string drawing, out string highestRatedUserAvatar)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                int userHighestRating = session.ConnectedUsers.Max(u => u.Ratings);
                highestRating = userHighestRating;
                highestRatingUsername = session.ConnectedUsers.FirstOrDefault(u => u.Ratings == userHighestRating).Username;
                drawing = session.ConnectedUsers.FirstOrDefault(u => u.Ratings == userHighestRating).Drawing;
                highestRatedUserAvatar = session.ConnectedUsers.FirstOrDefault(u => u.Ratings == userHighestRating).AvatarId;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void GetSessionHighestScore(int sessionId, out int highestScore, out string highestScoreUsername, out string highestScoreAvatar)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                int userHighestScore = session.ConnectedUsers.Max(u => u.Score);
                highestScore = userHighestScore;
                highestScoreUsername = session.ConnectedUsers
                    .FirstOrDefault(u => u.Score == userHighestScore).Username;
                highestScoreAvatar = session.ConnectedUsers
                    .FirstOrDefault(u => u.Score == userHighestScore).AvatarId;
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void SetUserFinalDrawing(int sessionId, string connectionId, string drawing)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                var user = session.ConnectedUsers.FirstOrDefault(u => u.ConnectionId == connectionId);
                if (user != null)
                {
                    user.Drawing = drawing;
                    Console.WriteLine($"User {user.Username}'s final drawing set in session {sessionId}.");
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

        public void RemoveUserDrawings(int sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                foreach (var user in session.ConnectedUsers)
                {
                    user.Drawing = string.Empty;
                }
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void IncrementUserScore(int sessionId, string username, int score)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                var user = session.ConnectedUsers.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    user.Score += score; // Update the user's score
                    Console.WriteLine($"User {user.Username}'s score updated to {score} in session {sessionId}.");

                    var formattedUsers = session.ConnectedUsers.Select(user => new SessionUserDto
                    {
                        Username = user.Username,
                        AvatarId = user.AvatarId,
                        Score = user.Score,
                    }).ToList();

                    UpdatePlayerLeaderboard?.Invoke(sessionId, formattedUsers);
                }
                else
                {
                    throw new KeyNotFoundException($"User with username {username} not found in session {sessionId}.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void IncrementSessionScores(int sessionId, int score, List<string>? exceptionUsers = null)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                foreach (var user in session.ConnectedUsers)
                {
                    if (exceptionUsers != null && exceptionUsers.Contains(user.Username))
                    {
                        continue; // Skip the user with the specified connection ID
                    }
                    user.Score += score; // Increment each user's score by the specified amount
                }
                Console.WriteLine($"Scores incremented by {score} for all users in session {sessionId}.");

                var formattedUsers = session.ConnectedUsers.Select(user => new SessionUserDto
                {
                    Username = user.Username,
                    AvatarId = user.AvatarId,
                    Score = user.Score,
                }).ToList();

                UpdatePlayerLeaderboard?.Invoke(sessionId, formattedUsers);
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void ResetGameSession(int sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.GameState = Model.Constants.DoodleChamp.PreGame;
                session.SelectedPrompt = string.Empty;
                session.UsersTurn = string.Empty;
                session.GuessedCorrectly = 0;
                session.GuessedUsers.Clear();
                session.ReleasedHintPositions.Clear();

                ResetUserScores(sessionId);
                ResetUsersRatings(sessionId);
                RemoveUserDrawings(sessionId);
                Console.WriteLine($"Game session {sessionId} has been reset.");
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }

        public void ResetGameRound(int sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.GameState =  Model.Constants.DoodleChamp.InGame;
                session.GuessedCorrectly = 0;
                session.GuessedUsers.Clear();
                session.ReleasedHintPositions.Clear();
                Console.WriteLine($"Game round for session {sessionId} has been reset.");
            }
            else
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }
        }
    }
}

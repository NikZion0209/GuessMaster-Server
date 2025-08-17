using GuessMaster.Data.Models;
using GuessMaster.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Interface
{
    public interface IDoodleChampRepository
    {
        public bool TryGetSession(int sessionId, out DoodleChampSession? session);
        public void CreateNewSession(out List<DoodleChampSession> doodleChampSession);
        public void RemoveSession(int sessionId);
        public void RemoveUserFromSession(int sessionId, string connectionId);
        public void GetSessionUsers(int sessionId, out List<ConnectedUser> users);
        public void GetSessionState(int sessionId, out int sessionState);
        public void UpdateSessionUsers(int sessionId, List<ConnectedUser> users);
        public void UpdateSessionState(int sessionId, int state);
        public bool CheckIfSessionExists(int sessionId);
        public void GetSessionPrompt(int sessionId, out string prompt);
        public void SetSessionPrompt(int sessionId, string prompt);
        public void GetSessionUsersTurn(int sessionId, out string username);
        public void ResetSessionUsersTurn(int sessionId);
        public void GetAvailableSessions(out List<DoodleChampSession> availableSessions);
        public void AddUserToSession(int sessionId, User user);
        public void UpdateUserConnectionId(int sessionId, int userId, string connectionId);
        public string GetUserNameByUserId(int sessionId, int userId);
        public string GetUserNameByConnectionId(int sessionId, string connectionId);
        public void GenerateOrderOfPlay(int sessionId);
        public void UpdatePlayerTurn(int sessionId, string username);
        public void IncrementGuessCount(int sessionId);
        public void ResetGuessCount(int sessionId);
        public void GetGuessCount(int sessionId, out int guessCount);
        public void GetPlayerCount(int sessionId, out int playerCount);
        public void GetCorrectUsers(int sessionId, out List<String> users);
        public void AddCorrectUser(int sessionId, string username);
        public void ResetCorrectUsers(int sessionId);
        public void GetConnectionIdByUsername(int sessionId, string username, out string connectionId);
        public void ResetReleasedHints(int sessionId);
        public void AddReleasedHintPosition(int sessionId, int position);
        public void GetReleasedHintPositions(int sessionId, out List<int> positions);
        public void ResetUserScores(int sessionId);
        public void IncrementUserScore(int sessionId, string username, int score);
        public void SetSessionUsersTurn(int sessionId, string username);
        public void IncrementSessionScores(int sessionId, int score, List<string>? exceptionUsers = null);
        public void ResetGameRound(int sessionId);
        public void ResetGameSession(int sessionId);
        public int AddUserToNextAvailableSession(User user, out int sessionId);
        public void ResetUsersRatings(int sessionId);
        public void IncrementUserRating(int sessionId, string username);
        public void GetSessionHighestRating(int sessionId, out string highestRatingUsername, out int highestRating, out string drawing);
        public void GetSessionHighestScore(int sessionId, out int highestScore, out string highestScoreUsername);
        public void RemoveUserDrawings(int sessionId);
        public void SetUserFinalDrawing(int sessionId, string connectionId, string drawing);
    }
}

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
        public void GetSessionUsersTurn(int sessionId, out string connectionId);
        public void ResetSessionUsersTurn(int sessionId);
        public void GetAvailableSessions(out List<DoodleChampSession> availableSessions);
        public void AddUserToSession(int sessionId, User user);
        public void UpdateUserConnectionId(int sessionId, int userId, string connectionId);
        public string GetUserNameByUserId(int sessionId, int userId);
        public string GetUserNameByConnectionId(int sessionId, string connectionId);
        public void GenerateOrderOfPlay(int sessionId);
        public void UpdatePlayerTurn(int sessionId, string username);
    }
}

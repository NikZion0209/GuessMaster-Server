using GuessMaster.Data.Models;
using GuessMaster.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Interface
{
    public interface IDoodleChamp
    {
        public Task StartGame(int sessionId);
        public void GenerateOrderOfPlay(int sessionId);
        public void RemoveFromSession(int sessionId, string connectionId);
        public void GetAvailableSessions(out List<DoodleChampSession> availableSessions);
        public void CreateNewSession(out List<DoodleChampSession> doodleChampSession);
        public void AddUserToSession(int sessionId, User user);
        public void UpdateUserConnectionId(int sessionId, int userId, string connectionId);
        public string GetUserNameById(int sessionId, int userId);
        public void CheckLobbyStatus(int sessionId);
        public void GetSessionUsers(int sessionId, out List<ConnectedUser> users);
    }
}

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
        public void RemoveFromSession(int sessionId, string connectionId);
        public void CheckLobbyStatus(int sessionId);
        public void ResolveUserGuess(int sessionId, string username, string guess);
        public void GetHintPosition(int sessionId, out int hintPosition, out char hintLetter);
    }
}

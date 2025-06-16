using GuessMaster.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Interface
{
    public interface IGameSession
    {
        List<GameSession> GetAvailableGameSessions(int gameType);
        List<GameSession> CreateNewSession(int gameType);
        GameSession? GetSessionById(int sessionId);
        void UpdateSession(GameSession session);
    }
}

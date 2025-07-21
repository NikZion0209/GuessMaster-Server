using GuessMaster.Data.Models;
using GuessMaster.Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GuessMaster.Service.Service.GameSessions;

namespace GuessMaster.Service.Interface
{
    public interface IGameSessions
    {
        public List<GameSessionDTO> GetAvailableGameSessions(int gameType);
        void AddUserToSession(int gameType, int sessionId, int userId);
    }
}

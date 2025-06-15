using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuessMaster.Data.Models;

namespace GuessMaster.Repository.Interface
{
    public interface IGameSession
    {
        List<GameSession> GetAvailableGameSessions(int gameType);
    }
}

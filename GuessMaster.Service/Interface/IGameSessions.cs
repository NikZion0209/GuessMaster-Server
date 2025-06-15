using GuessMaster.Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuessMaster.Data.Models;

namespace GuessMaster.Service.Interface
{
    public interface IGameSessions
    {
        List<GameSession> GetAvailableGameSessions(int gameType);
    }
}

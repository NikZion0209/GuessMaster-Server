using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuessMaster.Data.Data;
using GuessMaster.Data.Models;
using GuessMaster.Repository.Interface;

namespace GuessMaster.Repository.Repository
{
    public class GameSession : IGameSession
    {
        private readonly ApplicationDbContext _context;

        public GameSession(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Data.Models.GameSession> GetAvailableGameSessions(int gameType)
        {
            try
            {
                return _context.GameSessions
                    .Where(gs => !gs.IsFull && !gs.InPlay && gs.GameType == gameType)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving game sessions.", ex);
            }
        }
    }
}

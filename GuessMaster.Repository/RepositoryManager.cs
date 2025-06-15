using GuessMaster.Data.Data;
using GuessMaster.Repository.Interface;
using GuessMaster.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IPlayerRepository> _lazyPlayerRepository;
        private readonly Lazy<IGameSession> _lazyGameSessionRepository;

        public RepositoryManager(ApplicationDbContext _context)
        {
            _lazyPlayerRepository = new Lazy<IPlayerRepository>(() => new PlayerRepository(_context));
            _lazyGameSessionRepository = new Lazy<IGameSession>(() => new GameSession(_context));
        }
        public IPlayerRepository PlayerRepository => _lazyPlayerRepository.Value;
        public IGameSession GameSessionRepository => _lazyGameSessionRepository.Value;
    }
}

using GuessMaster.Data.Data;
using GuessMaster.Repository.Interface;
using GuessMaster.Repository.Repository;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly Lazy<ILeaderboardRepository> _lazyLeaderboardRepository;
        public RepositoryManager(ApplicationDbContext _context, IMemoryCache cache)
        {
            _lazyPlayerRepository = new Lazy<IPlayerRepository>(() => new PlayerRepository(_context, cache));
            _lazyLeaderboardRepository = new Lazy<ILeaderboardRepository>(() => new LeaderboardRepository(_context, cache));
        }
        public IPlayerRepository PlayerRepository => _lazyPlayerRepository.Value;
        public ILeaderboardRepository LeaderboardRepository => _lazyLeaderboardRepository.Value;
    }
}

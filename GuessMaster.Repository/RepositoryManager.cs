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
        public RepositoryManager(ApplicationDbContext _context, IMemoryCache cache)
        {
            _lazyPlayerRepository = new Lazy<IPlayerRepository>(() => new PlayerRepository(_context, cache));
        }
        public IPlayerRepository PlayerRepository => _lazyPlayerRepository.Value;
    }
}

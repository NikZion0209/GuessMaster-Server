using GuessMaster.Repository;
using GuessMaster.Repository.Interface;
using GuessMaster.Repository.Repository;
using GuessMaster.Service.Interface;
using GuessMaster.Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GuessMaster.Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IPlayerService> _lazyPlayerService;
        private readonly Lazy<IGameSessions> _lazyGameService;
        private readonly Lazy<IGameTimer> _lazyGameTimer;
        private readonly Lazy<IDoodleChamp> _lazyDoodleChamp;
        private readonly Lazy<IPasswordHasher> _lazyPasswordHasher;
        private readonly Lazy<IJWTHelper> _lazyJwtHelper;
        private readonly Lazy<ILeaderboardService> _lazyLeaderboardService;

        public ServiceManager(
            IRepositoryManager _repositoryManager, 
            IHttpContextAccessor _httpContextAccessor, 
            IDoodleChampRepository _doodleChampRepository, 
            IPasswordHasher _passwordHasher,
            IConfiguration _configuration
        ) {
            _lazyPlayerService = new Lazy<IPlayerService>(() => new PlayerService(_repositoryManager, _httpContextAccessor, _passwordHasher));
            _lazyGameTimer = new Lazy<IGameTimer>(() => new GameTimer());
            _lazyDoodleChamp = new Lazy<IDoodleChamp>(() => new DoodleChamp(GameTimer , _doodleChampRepository));
            _lazyGameService = new Lazy<IGameSessions>(() => new GameSessions(_repositoryManager, _doodleChampRepository));
            _lazyPasswordHasher = new Lazy<IPasswordHasher>(() => new PasswordHasher());
            _lazyJwtHelper = new Lazy<IJWTHelper>(() => new JWTHelper(_configuration));
            _lazyLeaderboardService = new Lazy<ILeaderboardService>(() => new LeaderboardService(_repositoryManager));
        }

        public IPlayerService PlayerService => _lazyPlayerService.Value;
        public IGameSessions GameService => _lazyGameService.Value;
        public IGameTimer GameTimer => _lazyGameTimer.Value;
        public IDoodleChamp DoodleChamp => _lazyDoodleChamp.Value;
        public IPasswordHasher PasswordHasher => _lazyPasswordHasher.Value;
        public IJWTHelper JWTHelper => _lazyJwtHelper.Value;
        public ILeaderboardService LeaderboardService => _lazyLeaderboardService.Value;
    }
}

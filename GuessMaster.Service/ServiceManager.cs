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

namespace GuessMaster.Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IPlayerService> _lazyPlayerService;
        private readonly Lazy<IGameSessions> _lazyGameService;
        private readonly Lazy<IGameTimer> _lazyGameTimer;
        private readonly Lazy<IDoodleChamp> _lazyDoodleChamp;

        public ServiceManager(IRepositoryManager _repositoryManager, IHttpContextAccessor _httpContextAccessor)
        {
            _lazyPlayerService = new Lazy<IPlayerService>(() => new PlayerService(_repositoryManager, _httpContextAccessor));
            _lazyGameTimer = new Lazy<IGameTimer>(() => new GameTimer());
            _lazyDoodleChamp = new Lazy<IDoodleChamp>(() => new DoodleChamp(GameTimer));
            _lazyGameService = new Lazy<IGameSessions>(() => new GameSessions(_repositoryManager, DoodleChamp));
        }

        public IPlayerService PlayerService => _lazyPlayerService.Value;
        public IGameSessions GameService => _lazyGameService.Value;
        public IGameTimer GameTimer => _lazyGameTimer.Value;
        public IDoodleChamp DoodleChamp => _lazyDoodleChamp.Value;
    }
}

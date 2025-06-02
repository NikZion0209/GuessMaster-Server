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

        public ServiceManager(IRepositoryManager _repositoryManager,IHttpContextAccessor _httpContextAccessor)
        {
            _lazyPlayerService = new Lazy<IPlayerService>(() => new PlayerService(_repositoryManager, _httpContextAccessor));
        }

        public IPlayerService PlayerService => _lazyPlayerService.Value;
    }
}

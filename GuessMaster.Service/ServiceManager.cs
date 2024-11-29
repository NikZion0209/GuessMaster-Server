using GuessMaster.Repository;
using GuessMaster.Repository.Interface;
using GuessMaster.Repository.Repository;
using GuessMaster.Service.Interface;
using GuessMaster.Service.Service;
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
        private readonly Lazy<IPlayerService> _lazyIPlayerService;

        public ServiceManager(IRepositoryManager _repositoryManager)
        {
            _lazyIPlayerService = new Lazy<IPlayerService>(() => new PlayerService(_repositoryManager));
        }

        public IPlayerService PlayerRepository => _lazyIPlayerService.Value;
    }
}

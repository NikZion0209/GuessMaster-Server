using GuessMaster.Data.Models;
using GuessMaster.Repository;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Service
{
    public class PlayerService : IPlayerService
    {
        private readonly IRepositoryManager _repositoryManager;

        public PlayerService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public bool AddPlayer(User user)
        {
            try
            {
               return _repositoryManager.PlayerRepository.AddPlayer(user);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<User> GetAllPlayers()
        {
            try
            {
               return _repositoryManager.PlayerRepository.GetAllPlayers();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool RemovePlayer(User user)
        {
            try
            {
               return _repositoryManager.PlayerRepository.RemovePlayer(user);   
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

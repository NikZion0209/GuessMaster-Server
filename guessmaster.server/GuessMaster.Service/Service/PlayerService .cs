using GuessMaster.Data.Models;
using GuessMaster.Repository;
using GuessMaster.Service.Interface;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PlayerService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor)
        {
            _repositoryManager = repositoryManager;
            _httpContextAccessor = httpContextAccessor; 
        }

        public User AddPlayer(User user)
        {
            try
            {
                return _repositoryManager.PlayerRepository.AddPlayer(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RoomAssignment> AddPlayerOrAssignmentRoomAsync(User user)
        {
            try
            {
                var player = _repositoryManager.PlayerRepository.AddPlayer(user);
                RoomService roomService = new RoomService(_repositoryManager, _httpContextAccessor);
                var room = await roomService.JoinOrCreateRoomAsync(player.UserId);
                var roomAssignment = new RoomAssignment()
                {
                    RoomId = room.RoomId,
                    UserId = user.UserId
                };
                return roomAssignment;
            }
            catch (Exception)
            {

                throw;
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

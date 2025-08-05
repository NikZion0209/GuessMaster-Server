using GuessMaster.Data.Models;
using GuessMaster.Model.ViewModel;
using GuessMaster.Repository;
using GuessMaster.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        private readonly IPasswordHasher _passwordHasher;

        public PlayerService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IPasswordHasher passwordHasher)
        {
            _repositoryManager = repositoryManager;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = passwordHasher;
        }

        public RegisterUserDto AddOrValidateUser(User user, out RegisterUserDto registerUserDto)
        {
            User? existingUser = _repositoryManager.PlayerRepository.GetUserByUsername(user.Username);
            if (existingUser == null)
            {
                // Username does not exist, hash and add
                user.Password = _passwordHasher.HashPassword(user.Password);
                existingUser = _repositoryManager.PlayerRepository.AddPlayer(user);
            }
            else
            {
                if (!_passwordHasher.VerifyPassword(user.Password, existingUser.Password))
                {
                    throw new UnauthorizedAccessException("Password is incorrect.");
                }
            }

            return registerUserDto = new RegisterUserDto
            {
                UserId = existingUser.UserId,
                Username = existingUser.Username,
                AvatarUrl = existingUser.AvatarUrl
            };
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

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

        public RegisterUserDto AddUser(RegistrationPostDto user, out RegisterUserDto registerUserDto)
        {
            User? newUser = _repositoryManager.PlayerRepository.GetUserByEmail(user.Email);
            if (newUser != null)
            {
                throw new UnauthorizedAccessException("Email already has account registered.");
            }

            if (_repositoryManager.PlayerRepository.GetUserByUsername(user.Username) != null)
            {
                throw new UnauthorizedAccessException("Username is already taken.");
            }
            user.Password = _passwordHasher.HashPassword(user.Password);
            newUser = _repositoryManager.PlayerRepository.AddPlayer(new User
            {
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
                AvatarId = user.AvatarId
            });

            return registerUserDto = new RegisterUserDto
            {
                UserId = newUser.UserId,
                Username = newUser.Username,
                AvatarId = newUser.AvatarId,
                PremiumTokens = newUser.PremiumToken
            };
        }

        public RegisterUserDto ValidateUser(RegistrationPostDto user, out RegisterUserDto registerUserDto)
        {
            User? existingUserByEmail = _repositoryManager.PlayerRepository.GetUserByEmail(user.LoginName);
            User? existingUserByUsername = _repositoryManager.PlayerRepository.GetUserByUsername(user.LoginName);

            User? existingUser = existingUserByEmail ?? existingUserByUsername;
            if (existingUser == null)
            {
                throw new ArgumentException("No account registered to this email/username");
            }

            if (!_passwordHasher.VerifyPassword(user.Password, existingUser.Password))
            {
                throw new UnauthorizedAccessException("Password is incorrect.");
            }

            return registerUserDto = new RegisterUserDto
            {
                UserId = existingUser.UserId,
                Username = existingUser.Username,
                AvatarId = existingUser.AvatarId,
                PremiumTokens = existingUser.PremiumToken
            };
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

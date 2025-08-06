using GuessMaster.Data.Data;
using GuessMaster.Data.Models;
using GuessMaster.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ApplicationDbContext _context;

        public PlayerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User AddPlayer(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return user;
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
                return _context.Users.ToList();
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
                _context.Users.Remove(user);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void UpdateUserTimestamps(User user)
        {
            user.UpdatedAt = DateTime.Now;
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public User GetUserById(int userId)
        {
            try
            {
                var user = _context.Users.Find(userId) ?? throw new Exception("User not found.");
                UpdateUserTimestamps(user);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the user.", ex);
            }
        }

        public User? GetUserByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                UpdateUserTimestamps(user);
            }
            return user;
        }

        public void UpdateUser(User user)
        {
            try
            {
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user.", ex);
            }
        }
    }
}

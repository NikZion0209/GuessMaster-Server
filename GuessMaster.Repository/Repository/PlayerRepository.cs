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
    }
}

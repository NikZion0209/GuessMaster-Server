using GuessMaster.Data.Data;
using GuessMaster.Data.Models;
using GuessMaster.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using GuessMaster.Model.ViewModel;


namespace GuessMaster.Repository.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public PlayerRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60), // Cache for 60 minutes
                SlidingExpiration = TimeSpan.FromMinutes(10) // Reset expiration if accessed
            };
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

        public bool RemovePlayer(User user)
        {
            try
            {
                _context.Users.Remove(user);
                _context.SaveChanges();

                // Invalidate cache entries for this user
                _cache.Remove($"UserId_{user.UserId}");
                _cache.Remove($"Username_{user.Username}");

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
            if (_cache.TryGetValue($"UserId_{userId}", out User cachedUser))
            {
                return cachedUser;
            }

            try
            {
                var user = _context.Users.Find(userId) ?? throw new Exception("User not found.");
                UpdateUserTimestamps(user);
                _cache.Set($"UserId_{userId}", user, _cacheOptions);
                _cache.Set($"Username_{user.Username}", user, _cacheOptions); // Also cache by username
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the user.", ex);
            }
        }

        public User? GetUserByUsername(string username)
        {
            if (_cache.TryGetValue($"Username_{username}", out User cachedUser))
            {
                return cachedUser;
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                UpdateUserTimestamps(user);
                _cache.Set($"Username_{username}", user, _cacheOptions);
                _cache.Set($"UserId_{user.UserId}", user, _cacheOptions); // Also cache by ID
            }
            return user;
        }

        public User? GetUserByEmail(string email)
        {
            if (_cache.TryGetValue($"Email_{email}", out User cachedUser))
            {
                return cachedUser;
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                UpdateUserTimestamps(user);
                _cache.Set($"Email_{user.Email}", user, _cacheOptions);
                _cache.Set($"UserId_{user.UserId}", user, _cacheOptions); // Also cache by ID
            }
            return user;
        }

        public void SetHighscore(int userId, int gameType, int score)
        {
            try
            {
                var user = GetUserById(userId);
                switch (gameType)
                {
                    case 1: // Doodle Champ
                        if (score > user.HighscoreDoodleChamp)
                        {
                            user.HighscoreDoodleChamp = score;
                        }
                        break;
                    case 2: // Flag Whiz
                        if (score > user.HighscoreFlagWhiz)
                        {
                            user.HighscoreFlagWhiz = score;
                        }
                        break;
                    case 3: // Word Snap
                        if (score > user.HighscoreWordSnap)
                        {
                            user.HighscoreWordSnap = score;
                        }
                        break;
                    default:
                        throw new ArgumentException("Invalid game type specified.");
                }
                UpdateUserTimestamps(user);
                _context.Users.Update(user);
                _context.SaveChanges();
                // Invalidate cache entries for this user
                _cache.Remove($"UserId_{user.UserId}");
                _cache.Remove($"Username_{user.Username}");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while setting the high score.", ex);
            }
        }

        public HighScores GetAllHighscores(int userId)
        {
            try
            {
                var user = GetUserById(userId);
                return new HighScores
                {
                    HighscoreDoodleChamp = user.HighscoreDoodleChamp,
                    HighscoreFlagWhiz = user.HighscoreFlagWhiz,
                    HighscoreWordSnap = user.HighscoreWordSnap
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving highscores.", ex);
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                _context.Users.Update(user);
                _context.SaveChanges();

                // Invalidate cache entries for this user
                _cache.Remove($"UserId_{user.UserId}");
                _cache.Remove($"Username_{user.Username}");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user.", ex);
            }
        }
    }
}

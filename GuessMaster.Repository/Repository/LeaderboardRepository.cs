using GuessMaster.Data.Data;
using GuessMaster.Model.Models;
using GuessMaster.Model.ViewModel;
using GuessMaster.Repository.Interface;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Repository
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public LeaderboardRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60), // Cache for 60 minutes, refresh every hour
            };
        }

        public List<SessionUserDto> GetTopTenPlayers(int gameType)
        {
            try
            {
                string cacheKey = $"TopTenPlayers_{gameType}";
                if (!_cache.TryGetValue(cacheKey, out List<SessionUserDto> topPlayers))
                {
                    topPlayers = _context.Leaderboards
                        .Where(l => l.Gamemode == gameType)
                        .OrderByDescending(l => l.Score)
                        .Take(10)
                        .Select(l => new SessionUserDto
                        {
                            Username = l.Username,
                            AvatarId = l.AvatarId,
                            Score = l.Score
                        })
                        .ToList();
                    _cache.Set(cacheKey, topPlayers, _cacheOptions);
                }
                return topPlayers;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddLeaderboardEntry(Leaderboards entry)
        {
            try
            {
                var existingEntry = _context.Leaderboards
                    .FirstOrDefault(l => l.Username == entry.Username && l.Gamemode == entry.Gamemode);

                if (existingEntry != null)
                {
                    if (entry.Score > existingEntry.Score)
                    {
                        existingEntry.Score = entry.Score;
                        existingEntry.AvatarId = entry.AvatarId;
                        _context.Leaderboards.Update(existingEntry);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    _context.Leaderboards.Add(entry);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GetPlayerRank(int gameType, string username, out int rank, out int score)
        {
            rank = -1;
            score = 0;
            try
            {
                var playerEntry = _context.Leaderboards
                    .FirstOrDefault(l => l.Username == username && l.Gamemode == gameType);
                if (playerEntry != null)
                {
                    score = playerEntry.Score;
                    rank = _context.Leaderboards
                        .Where(l => l.Gamemode == gameType && l.Score > playerEntry.Score)
                        .Count() + 1;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ClearLeaderboard(int gameType)
        {
            string cacheKey = $"TopTenPlayers_{gameType}";
            _cache.Remove(cacheKey);
            var entries = _context.Leaderboards.Where(l => l.Gamemode == gameType).ToList();
            _context.Leaderboards.RemoveRange(entries);
        }

        public void ClearAllLeaderboards()
        {
            var gameTypes = _context.Leaderboards.Select(l => l.Gamemode).Distinct().ToList();
            foreach (var gameType in gameTypes)
            {
                string cacheKey = $"TopTenPlayers_{gameType}";
                _cache.Remove(cacheKey);
            }

            var allEntries = _context.Leaderboards.ToList();
            _context.Leaderboards.RemoveRange(allEntries);
        }
    }
}

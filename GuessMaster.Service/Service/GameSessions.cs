using GuessMaster.Data.Models;
using GuessMaster.Model.Constants;
using GuessMaster.Model.Models;
using GuessMaster.Model.ViewModel;
using GuessMaster.Repository;
using GuessMaster.Repository.Interface;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Cryptography;

namespace GuessMaster.Service.Service
{
    public class GameSessions : IGameSessions
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IDoodleChampRepository _doodleChampRepository;

        // AES key and IV (should be kept secret and only in this class)
        private static readonly byte[] _aesKey = RandomNumberGenerator.GetBytes(32);
        private static readonly byte[] _aesIV = RandomNumberGenerator.GetBytes(16);

        public GameSessions(IRepositoryManager repositoryManager, IDoodleChampRepository doodleChampRepository)
        {
            _repositoryManager = repositoryManager;
            _doodleChampRepository = doodleChampRepository;
        }

        public class GameSessionDTO
        {
            public int SessionId { get; set; }
            public int PlayerCount { get; set; }
            public int MaxPlayers { get; set; }
        }

        public List<GameSessionDTO> GetAvailableGameSessions(int gameType)
        {
            try
            {
                switch (gameType)
                {
                    case Gamemodes.DoodleChamp:
                        _doodleChampRepository.GetAvailableSessions(out var availableSessions);

                        if (availableSessions == null || !availableSessions.Any())
                        {
                            _doodleChampRepository.CreateNewSession(out availableSessions);
                        }

                        return availableSessions.Select(s => new GameSessionDTO
                        {
                            SessionId = s.SessionId,
                            PlayerCount = s.PlayerCount,
                            MaxPlayers = s.MaxPlayers
                        }).ToList();

                    default:
                        throw new ArgumentException("Invalid game type specified.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while retrieving game sessions.", ex);
            }
        }

        public void AddUserToSession(int gameType, int sessionId, int userId)
        {
            try
            {
                User user = _repositoryManager.PlayerRepository.GetUserById(userId);
                switch (gameType)
                {
                    case Gamemodes.DoodleChamp:
                        _doodleChampRepository.AddUserToSession(sessionId, user);
                        break;
                    default:
                        throw new ArgumentException("Invalid game type specified.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                Console.WriteLine($"Error adding user to session: {ex.Message}");
                throw new Exception("An error occurred while adding user to session.", ex);
            }
        }

        public int AddUserToNextAvailableSession(int gameType, int userId, out int sessionId)
        {
            try
            {
                User user = _repositoryManager.PlayerRepository.GetUserById(userId);
                switch (gameType)
                {
                    case Gamemodes.DoodleChamp:
                        _doodleChampRepository.AddUserToNextAvailableSession(user, out int doodleChampSessionId);
                        sessionId = doodleChampSessionId;
                        break;
                    default:
                        throw new ArgumentException("Invalid game type specified.");
                }
                return sessionId;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                Console.WriteLine($"Error adding user to session: {ex.Message}");
                throw new Exception("An error occurred while adding user to session.", ex);
            }
        }

        public string GenerateSinglePlayerSession(SinglePlayerSessionData sessionData)
        {
            sessionData.SessionId = Convert.ToBase64String(RandomNumberGenerator.GetBytes(8));
            string json = JsonSerializer.Serialize(sessionData);
            return Encrypt(json);
        }

        public void DecryptSinglePlayerSession(int gameType, int score, SinglePlayerSessionData currentSessionData, string encrypted)
        {
            try
            {
                string json = Decrypt(encrypted);
                SinglePlayerSessionData sessionData = JsonSerializer.Deserialize<SinglePlayerSessionData>(json);

                // Check if the session has expired (valid for 1 hour)
                if (sessionData == null || (DateTime.UtcNow - sessionData.IssuedAt).TotalHours > 1)
                {
                    throw new Exception("Session has expired or is invalid.");
                }

                // Check if the game type matches
                if (sessionData.GameType != gameType)
                {
                    throw new Exception("Game type does not match the session data.");
                }

                // Check if the user ID matches
                if (sessionData.UserId != currentSessionData.UserId)
                {
                    throw new Exception("User ID does not match the session data.");
                }

                // Check if the IP address matches
                if (sessionData.IpAddress != currentSessionData.IpAddress)
                {
                    throw new Exception("IP address does not match the session data.");
                }

                // Check if the User Agent matches
                if (sessionData.UserAgent != currentSessionData.UserAgent)
                {
                    throw new Exception("User Agent does not match the session data.");
                }

                int maxScore = gameType switch
                {
                    Gamemodes.FlagWhiz => 100 + (int)(20 * (DateTime.UtcNow - sessionData.IssuedAt).TotalSeconds),
                    Gamemodes.WordSnap => 100 + (int)(40 * (DateTime.UtcNow - sessionData.IssuedAt).TotalSeconds),
                    _ => throw new ArgumentException("Invalid game type specified.")
                };

                // Validate the score
                if (score < 0 || score > maxScore)
                {
                    throw new Exception("Score is out of valid range.");
                }

                _repositoryManager.PlayerRepository.SetHighscore(int.Parse(sessionData.UserId), gameType, score);
                _repositoryManager.LeaderboardRepository.AddLeaderboardEntry(new Leaderboards
                {
                    Gamemode = gameType,
                    Username = _repositoryManager.PlayerRepository.GetUserById(int.Parse(sessionData.UserId)).Username,
                    AvatarId = _repositoryManager.PlayerRepository.GetUserById(int.Parse(sessionData.UserId)).AvatarId,
                    Score = score
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting session data: {ex.Message}");
                throw new Exception("An error occurred while decrypting session data.", ex);
            }
        }

        // AES encryption
        private string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _aesKey;
            aes.IV = _aesIV;
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        // AES decryption
        private string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _aesKey;
            aes.IV = _aesIV;
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}

using GuessMaster.Model.Models;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Service
{
    public class DoodleChamp : IDoodleChamp
    {
        private readonly IGameTimer _gameTimer;

        public static event Action<int>? StartingLobbyTimer;
        public static event Action<int>? StoppingLobbyTimer;
        public static event Action<int>? GameStarted;
        public static event Action<int>? GameEnded;

        public DoodleChamp(IGameTimer gameTimer)
        {
            _gameTimer = gameTimer;
        }

        public async Task StartGame(int sessionId, List<ConnectedUser> Users)
        {
            Console.WriteLine($"Starting Lobby timer for session {sessionId} with {Users.Count} users.");
            StartingLobbyTimer?.Invoke(sessionId);
            await _gameTimer.StartTimer(sessionId, Model.Constants.DoodleChamp.LobbyTimer, Model.Constants.DoodleChamp.LobbyCountdown, Model.Constants.Gamemodes.DoodleChamp);
            GameStarted?.Invoke(sessionId);
        }
    }
}

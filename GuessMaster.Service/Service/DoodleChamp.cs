using GuessMaster.Data.Models;
using GuessMaster.Model.Constants;
using GuessMaster.Model.Models;
using GuessMaster.Repository;
using GuessMaster.Repository.Interface;
using GuessMaster.Service.Interface;
using System.Text.Json;

namespace GuessMaster.Service.Service
{
    public class DoodleChamp : IDoodleChamp
    {
        private readonly IGameTimer _gameTimer;
        private readonly IDoodleChampRepository _doodleChampRepository;

        public static event Action<int>? StartingLobbyTimer;
        public static event Action<int>? StoppingLobbyTimer;
        public static event Action<int>? GameStarted;
        public static event Action<int>? GameRestart;
        public static event Action<int>? GameEndedEarly;
        public static event Action<int, List<User>>? UpdatePlayerLeaderboard;
        public static event Action<int, string, string>? NotifyUserTurn;
        public static event Action<string>? NotifyEndUserTurn;
        public static event Action<int, string, List<string>>? SendGeneratedPrompts;
        public static event Action<int, string>? NotifyPromptSelectionEnd;
        public static event Action<int, string>? NotifyWholeSession;
        public static event Action<int, string, string>? NotifyUserInSession;
        public static event Action<int, bool>? ToggleSessionGuessAbility;

        public DoodleChamp(IGameTimer gameTimer, IDoodleChampRepository doodleChampRepository)
        {
            _gameTimer = gameTimer;
            _doodleChampRepository = doodleChampRepository;
        }

        public void CheckLobbyStatus(int sessionId)
        {
            _doodleChampRepository.TryGetSession(sessionId, out var session);
            if (session == null)
            {
                throw new KeyNotFoundException($"Session {sessionId} not found.");
            }

            if (session.ConnectedUsers.Count >= Model.Constants.DoodleChamp.MinPlayers)
            {
                switch (session.GameState)
                {
                    case Model.Constants.DoodleChamp.PreGame:
                        Console.WriteLine($"Session {sessionId} has enough players to start the game.");
                        StartGame(sessionId);
                        break;
                    case Model.Constants.DoodleChamp.Lobby:
                        Console.WriteLine($"Session {sessionId} is already in the lobby state.");
                        StartingLobbyTimer?.Invoke(sessionId);
                        _doodleChampRepository.GenerateOrderOfPlay(sessionId);
                        break;
                    default:
                        return;
                }

            }

        }

        public async Task StartGame(int sessionId)
        {
            _doodleChampRepository.UpdateSessionState(sessionId, Model.Constants.DoodleChamp.Lobby);
            StartingLobbyTimer?.Invoke(sessionId);

            await _gameTimer.StartTimer(
                sessionId,
                Model.Constants.DoodleChamp.LobbyTimer,
                Model.Constants.DoodleChamp.LobbyCountdown,
                Gamemodes.DoodleChamp,
                Model.Constants.DoodleChamp.OrderOfPlayCountdown,
                Model.Constants.DoodleChamp.OrderOfPlayList
            );

            _doodleChampRepository.UpdateSessionState(sessionId, Model.Constants.DoodleChamp.InGame);
            GameStarted?.Invoke(sessionId);

            _doodleChampRepository.GetSessionUsers(sessionId, out var users);

            foreach (var user in users)
            {
                _doodleChampRepository.UpdatePlayerTurn(sessionId, user.Username);
                NotifyUserTurn?.Invoke(sessionId, user.ConnectionId, user.Username);

                _doodleChampRepository.GetSessionPrompt(sessionId, out string prompt);
                List<string> prompts = GeneratePrompts(prompt);
                _doodleChampRepository.SetSessionPrompt(sessionId, prompts[0]); // First prompt is default for the round
                SendGeneratedPrompts?.Invoke(sessionId, user.ConnectionId, prompts);

                // Select a prompt timer
                try
                {
                    await _gameTimer.StartTimer(
                        sessionId,
                        Model.Constants.DoodleChamp.SelectionTimer,
                        Model.Constants.DoodleChamp.SelectionCountDown,
                        Gamemodes.DoodleChamp
                    );
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine($"Timer for session {sessionId} was canceled.");
                    continue; // Exit if the timer was canceled
                }

                NotifyPromptSelectionEnd?.Invoke(sessionId, user.ConnectionId);
                ToggleSessionGuessAbility?.Invoke(sessionId, true);

                // Drawing timer
                try
                {
                    await _gameTimer.StartTimer(
                        sessionId,
                        Model.Constants.DoodleChamp.DrawingTimer,
                        Model.Constants.DoodleChamp.DrawingCountdown,
                        Gamemodes.DoodleChamp
                    );
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine($"Timer for session {sessionId} was canceled.");
                    continue; // Exit if the timer was canceled
                }

                ToggleSessionGuessAbility?.Invoke(sessionId, false);
                _doodleChampRepository.UpdateSessionState(sessionId, Model.Constants.DoodleChamp.RoundSummary);
                NotifyEndUserTurn?.Invoke(user.ConnectionId);

                // Round summary timer
                try
                {
                    await _gameTimer.StartTimer(
                        sessionId,
                        Model.Constants.DoodleChamp.RoundSummaryTimer,
                        Model.Constants.DoodleChamp.RoundSummaryCountdown,
                        Gamemodes.DoodleChamp
                    );
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine($"Timer for session {sessionId} was canceled.");
                    continue; // Exit if the timer was canceled
                }

                _doodleChampRepository.UpdateSessionState(sessionId, Model.Constants.DoodleChamp.InGame);
            }
        }

        public void RemoveFromSession(int sessionId, string connectionId)
        {
            _doodleChampRepository.RemoveUserFromSession(sessionId, connectionId);

            _doodleChampRepository.GetSessionUsers(sessionId, out List<ConnectedUser> currentUsers);
            _doodleChampRepository.GetSessionState(sessionId, out int gameState);

            if (
                currentUsers.Count < Model.Constants.DoodleChamp.MinPlayers &&
                (gameState == Model.Constants.DoodleChamp.Lobby || gameState == Model.Constants.DoodleChamp.InGame)
            )
            {
                Console.WriteLine($"Not enough players in session {sessionId}.");
                _gameTimer.PauseTimer(sessionId);

                switch (gameState)
                {
                    case Model.Constants.DoodleChamp.Lobby:
                        Console.WriteLine($"Lobby timer paused for session {sessionId} due to insufficient players.");
                        int time = _gameTimer.GetTimerLength(sessionId);

                        if (time < Model.Constants.DoodleChamp.QuickLobbyCountdown)
                        {
                            _gameTimer.SetTimerLength(sessionId, Model.Constants.DoodleChamp.QuickLobbyCountdown);
                        }
                        GameRestart?.Invoke(sessionId);
                        _doodleChampRepository.UpdateSessionState(sessionId, Model.Constants.DoodleChamp.PreGame);
                        break;
                    case Model.Constants.DoodleChamp.InGame:
                        Console.WriteLine($"Game ended for session {sessionId} due to insufficient players.");
                        _gameTimer.CancelTimer(sessionId);
                        GameEndedEarly?.Invoke(sessionId);
                        break;
                    default:
                        Console.WriteLine($"Unknown game state for session {sessionId}.");
                        break;
                }
                return;
            }

            var formattedUsers = currentUsers.Select(user => new User
            {
                Username = user.Username,
                AvatarUrl = user.AvatarUrl
            }).ToList();
            UpdatePlayerLeaderboard?.Invoke(sessionId, formattedUsers);

            _doodleChampRepository.GetSessionUsersTurn(sessionId, out string currentTurnConnectionId);

            if (gameState == Model.Constants.DoodleChamp.InGame && connectionId == currentTurnConnectionId)
            {
                _gameTimer.CancelTimer(sessionId);
                _doodleChampRepository.ResetSessionUsersTurn(sessionId);
            }
        }

        private static List<string> GeneratePrompts(string previousPrompt)
        {
            string filePath = Model.Constants.DoodleChamp.PromptFilePath;

            if (!File.Exists(filePath))
                throw new FileNotFoundException("WordBank.json not found.", filePath);

            try
            {
                var json = File.ReadAllText(filePath);
                var words = JsonSerializer.Deserialize<List<string>>(json);

                var random = new Random();
                var candidates = words;

                // Only filter out previousPrompt if it is not null or empty
                if (!string.IsNullOrWhiteSpace(previousPrompt))
                {
                    candidates = candidates.Where(word => word != previousPrompt).ToList();
                }

                return candidates.OrderBy(_ => random.Next())
                                 .Take(Model.Constants.DoodleChamp.DisplayedPrompts)
                                 .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading or parsing WordBank.json", ex);
            }
        }

        public void ResolveUserGuess(int sessionId, string username, string guess)
        {
            _doodleChampRepository.GetSessionPrompt(sessionId, out string prompt);
            _doodleChampRepository.GetCorrectUsers(sessionId, out List<string> correctUsers);

            //Correct Guess Logic
            if (string.Equals(prompt, guess, StringComparison.OrdinalIgnoreCase))
            {
                if (!correctUsers.Contains(username))
                {
                    _doodleChampRepository.AddCorrectUser(sessionId, username);
                    _doodleChampRepository.IncrementGuessCount(sessionId);
                    NotifyWholeSession?.Invoke(sessionId, $"{username} has guessed the word!");
                    OnCorrectGuess(sessionId);
                    return;
                }
                // User has already guessed correctly
                _doodleChampRepository.GetConnectionIdByUsername(sessionId, username, out string connectionId);
                NotifyUserInSession?.Invoke(sessionId, connectionId, "You have already guessed correctly!");
                return;
            }
            //Incorrect Guess Logic
            NotifyWholeSession?.Invoke(sessionId, $"{username} : {guess}");
            return;
        }

        private void OnCorrectGuess(int sessionId)
        {
            _doodleChampRepository.GetGuessCount(sessionId, out int guessCount);
            _doodleChampRepository.GetPlayerCount(sessionId, out int playerCount);
            _doodleChampRepository.GetSessionState(sessionId, out int gameState);

            if ((guessCount >= playerCount - 1) && (gameState == Model.Constants.DoodleChamp.InGame))
            {
                _gameTimer.CancelTimer(sessionId);
            }
        }
    }
}

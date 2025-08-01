using GuessMaster.Data.Models;
using GuessMaster.Model.Constants;
using GuessMaster.Model.Models;
using GuessMaster.Repository;
using GuessMaster.Repository.Interface;
using GuessMaster.Service.Interface;
using GuessMaster.Service.Service;
using Microsoft.AspNetCore.SignalR;

namespace GuessMaster.Service.Event_Handlers
{
    public class DoodleChampEventHandler : IEventHandler
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IDoodleChamp _doodleChamp;
        private readonly IDoodleChampRepository _doodleChampRepository;

        public DoodleChampEventHandler(IHubContext<ChatHub> hubContext,
            IDoodleChamp doodleChamp,
            IDoodleChampRepository doodleChampRepository
        )
        {
            _hubContext = hubContext;
            _doodleChamp = doodleChamp;
            _doodleChampRepository = doodleChampRepository;
        }

        public void Subscribe()
        {
            ChatHub.UserJoinedRoom += OnUserJoinedRoom;
            ChatHub.UserLeftRoom += OnUserLeftRoom;
            ChatHub.SaveDrawingPrompt += OnSaveDrawingPrompt;
            ChatHub.ResolveUserGuess += OnResolveUserGuess;

            GameTimer.TimerTick += OnTimerTick;
            GameTimer.DCLookoutConditionAction += OnLookoutCondition;

            Repository.Repository.DoodleChampRepository.UpdatePlayerLeaderboard += OnUpdatePlayerLeaderboard;

            Service.DoodleChamp.StartingLobbyTimer += OnStartingLobbyTimer;
            Service.DoodleChamp.GameStarted += OnGameStart;
            Service.DoodleChamp.GameRestart += OnGameRestart;
            Service.DoodleChamp.GameEndedEarly += OnGameEndEarly;
            Service.DoodleChamp.UpdatePlayerLeaderboard += OnUpdatePlayerLeaderboard;
            Service.DoodleChamp.NotifyUserTurn += OnUserTurn;
            Service.DoodleChamp.NotifyEndUserTurn += OnEndUserTurn;
            Service.DoodleChamp.SendGeneratedPrompts += OnSendGeneratedPrompts;
            Service.DoodleChamp.NotifyPromptSelectionEnd += OnEndPromptSelection;
            Service.DoodleChamp.NotifyWholeSession += OnNotifySession;
            Service.DoodleChamp.NotifyUserInSession += OnNotifyUserInSession;
            Service.DoodleChamp.ToggleSessionGuessAbility += OnGuessAbility;
            Service.DoodleChamp.ReleaseHintLength += OnReleaseHintLength;
            Service.DoodleChamp.ToggleRoundSummaryOverlay += OnToggleRoundSummaryOverlay;
        }

        public void Unsubscribe()
        {
            ChatHub.UserJoinedRoom -= OnUserJoinedRoom;
            ChatHub.UserLeftRoom -= OnUserLeftRoom;
            ChatHub.SaveDrawingPrompt -= OnSaveDrawingPrompt;
            ChatHub.ResolveUserGuess -= OnResolveUserGuess;

            GameTimer.TimerTick -= OnTimerTick;
            GameTimer.DCLookoutConditionAction -= OnLookoutCondition;

            Repository.Repository.DoodleChampRepository.UpdatePlayerLeaderboard -= OnUpdatePlayerLeaderboard;

            Service.DoodleChamp.StartingLobbyTimer -= OnStartingLobbyTimer;
            Service.DoodleChamp.GameStarted -= OnGameStart;
            Service.DoodleChamp.GameRestart -= OnGameRestart;
            Service.DoodleChamp.GameEndedEarly -= OnGameEndEarly;
            Service.DoodleChamp.UpdatePlayerLeaderboard -= OnUpdatePlayerLeaderboard;
            Service.DoodleChamp.NotifyUserTurn -= OnUserTurn;
            Service.DoodleChamp.NotifyEndUserTurn -= OnEndUserTurn;
            Service.DoodleChamp.SendGeneratedPrompts -= OnSendGeneratedPrompts;
            Service.DoodleChamp.NotifyPromptSelectionEnd -= OnEndPromptSelection;
            Service.DoodleChamp.NotifyWholeSession -= OnNotifySession;
            Service.DoodleChamp.NotifyUserInSession -= OnNotifyUserInSession;
            Service.DoodleChamp.ToggleSessionGuessAbility -= OnGuessAbility;
            Service.DoodleChamp.ReleaseHintLength -= OnReleaseHintLength;
            Service.DoodleChamp.ToggleRoundSummaryOverlay -= OnToggleRoundSummaryOverlay;
        }

        private void OnUserJoinedRoom(int sessionId, int userId, string connectionId)
        {
            _doodleChampRepository.UpdateUserConnectionId(sessionId, userId, connectionId);
            _doodleChampRepository.GetSessionUsers(sessionId, out List<ConnectedUser> users);

            var formattedUsers = users.Select(user => new ConnectedUser
            {
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                Score = user.Score,
            }).ToList();

            OnUpdatePlayerLeaderboard(sessionId, formattedUsers);
            string user = _doodleChampRepository.GetUserNameByUserId(sessionId, userId);
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.RoomMessage, $"{user} has joined the room");

            _doodleChamp.CheckLobbyStatus(sessionId);
        }

        private void OnUserLeftRoom(int sessionId, string connectionId)
        {
            Console.WriteLine($"User with connection ID {connectionId} left the room {sessionId}");
            if (_doodleChampRepository.CheckIfSessionExists(sessionId))
            {
                string user = _doodleChampRepository.GetUserNameByConnectionId(sessionId, connectionId);
                _hubContext.Clients.Group(sessionId.ToString())
                    .SendAsync(ChatEventNames.RoomMessage, $"{user} has left the room");
                _doodleChamp.RemoveFromSession(sessionId, connectionId);
            }
        }

        private void OnUpdatePlayerLeaderboard(int sessionId, List<ConnectedUser> users)
        {
            Console.WriteLine($"Notifiying users in session {sessionId} for UpdatePlayerLeaderboard event handler");
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.PlayersInSession, users);
        }

        private void OnTimerTick(int sessionId, int secondsLeft)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.TimerUpdate, secondsLeft);
        }

        private void OnStartingLobbyTimer(int sessionId)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.LobbyTimerStarted, true);
        }

        private void OnGameStart(int sessionId)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.GameState, true);
        }

        private void OnGameRestart(int sessionId)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.LobbyTimerStarted, false);
        }

        private async void OnGameEndEarly(int sessionId)
        {
            Console.WriteLine($"Notifiying users in session {sessionId} for GameEndEarly");
            await _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.GameEndEarly);

            if (ChatHub.SessionUsers.TryGetValue(sessionId, out var connectionIds))
            {
                Console.WriteLine($"Removing users from session {sessionId} in GameEndEarly event handler");
                foreach (var connectionId in connectionIds)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, sessionId.ToString());
                }
                ChatHub.SessionUsers.TryRemove(sessionId, out _);
            }

            Console.WriteLine($"Removing session {sessionId} from DoodleChamp in GameEndEarly event handler");
            _doodleChampRepository.RemoveSession(sessionId);
        }

        private void OnUserTurn(int sessionId, string connectionId, string username)
        {
            _hubContext.Clients.Client(connectionId)
                .SendAsync(ChatEventNames.UserTurn, true);

            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.RoomMessage, $"{username} is the drawer");
        }

        private void OnEndUserTurn(string connectionId)
        {
            _hubContext.Clients.Client(connectionId)
                .SendAsync(ChatEventNames.UserTurn, false);
        }

        private void OnLookoutCondition(int sessionId, string lookoutCondition)
        {
            if (lookoutCondition == Model.Constants.DoodleChamp.OrderOfPlayList)
            {
                _doodleChampRepository.GenerateOrderOfPlay(sessionId);
            }
            if (lookoutCondition == Model.Constants.DoodleChamp.ReleaseHint)
            {
                ReleaseHint(sessionId);
            }
        }

        private void OnSendGeneratedPrompts(int sessionId, string connectionId, List<string> prompts)
        {
            _hubContext.Clients.Client(connectionId)
                .SendAsync(ChatEventNames.ReceiveGeneratedPrompts, prompts);
        }

        private void OnEndPromptSelection(int sessionId, string connectionId)
        {
            _hubContext.Clients.Client(connectionId)
                .SendAsync(ChatEventNames.PromptSelectionEnd);
        }

        private void OnSaveDrawingPrompt(int sessionId, string prompt)
        {
            _doodleChampRepository.SetSessionPrompt(sessionId, prompt);
        }

        private void OnResolveUserGuess(int sessionId, string username, string guess)
        {
            _doodleChamp.ResolveUserGuess(sessionId, username, guess);
        }

        private void OnNotifySession(int sessionId, string message)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.RoomMessage, message);
        }

        private void OnNotifyUserInSession(int sessionId, string connectionId, string message)
        {
            _hubContext.Clients.Client(connectionId)
                .SendAsync(ChatEventNames.RoomMessage, message);
        }

        private void OnGuessAbility(int sessionId, bool canGuess)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.GuessingToggle, canGuess);
        }

        private void OnReleaseHintLength(int sessionId, int length)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.ReleaseHintLength, length);
        }

        private void ReleaseHint(int sessionId)
        {
            _doodleChamp.GetHintPosition(sessionId, out int hintPosition, out char hintLetter);
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.ReleaseHint, hintPosition, hintLetter);
        }

        private void OnToggleRoundSummaryOverlay(int sessionId, bool isVisible)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.RoundSummaryOverlay, isVisible);
        }
    }
}
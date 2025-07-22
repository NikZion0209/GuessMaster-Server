using GuessMaster.Data.Models;
using GuessMaster.Model.Constants;
using GuessMaster.Model.Models;
using GuessMaster.Service.Interface;
using GuessMaster.Service.Service;
using Microsoft.AspNetCore.SignalR;

namespace GuessMaster.Service.Event_Handlers
{
    public class DoodleChampEventHandler : IEventHandler
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IDoodleChamp _doodleChamp;

        public DoodleChampEventHandler(IHubContext<ChatHub> hubContext, IDoodleChamp doodleChamp)
        {
            _hubContext = hubContext;
            _doodleChamp = doodleChamp;
        }

        public void Subscribe()
        {
            ChatHub.UserJoinedRoom += OnUserJoinedRoom;
            ChatHub.UserLeftRoom += OnUserLeftRoom;

            GameTimer.TimerTick += OnTimerTick;
            GameTimer.DCLookoutConditionAction += OnLookoutCondition;

            Service.DoodleChamp.StartingLobbyTimer += OnStartingLobbyTimer;
            Service.DoodleChamp.GameStarted += OnGameStart;
            Service.DoodleChamp.GameRestart += OnGameRestart;
            Service.DoodleChamp.GameEndedEarly += OnGameEndEarly;
            Service.DoodleChamp.UpdatePlayerLeaderboard += OnUpdatePlayerLeaderboard;
            Service.DoodleChamp.NotifyUserTurn += OnUserTurn;
        }

        public void Unsubscribe()
        {
            ChatHub.UserJoinedRoom -= OnUserJoinedRoom;
            ChatHub.UserLeftRoom -= OnUserLeftRoom;

            GameTimer.TimerTick -= OnTimerTick;
            GameTimer.DCLookoutConditionAction -= OnLookoutCondition;

            Service.DoodleChamp.StartingLobbyTimer -= OnStartingLobbyTimer;
            Service.DoodleChamp.GameStarted -= OnGameStart;
            Service.DoodleChamp.GameRestart -= OnGameRestart;
            Service.DoodleChamp.GameEndedEarly -= OnGameEndEarly;
            Service.DoodleChamp.UpdatePlayerLeaderboard -= OnUpdatePlayerLeaderboard;
            Service.DoodleChamp.NotifyUserTurn -= OnUserTurn;
        }

        private void OnUserJoinedRoom(int sessionId, int userId, string connectionId)
        {
            _doodleChamp.UpdateUserConnectionId(sessionId, userId, connectionId);
            _doodleChamp.GetSessionUsers(sessionId, out List<ConnectedUser> users);

            var formattedUsers = users.Select(user => new User
            {
                Username = user.Username,
                AvatarUrl = user.AvatarUrl
            }).ToList();

            OnUpdatePlayerLeaderboard(sessionId, formattedUsers);
            string user = _doodleChamp.GetUserNameByUserId(sessionId, userId);
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.RoomMessage, $"{user} has joined the room");

            _doodleChamp.CheckLobbyStatus(sessionId);
        }

        private void OnUserLeftRoom(int sessionId, string connectionId)
        {
            Console.WriteLine($"User with connection ID {connectionId} left the room {sessionId}");
            if (_doodleChamp.CheckIfSessionExists(sessionId))
            {
                string user = _doodleChamp.GetUserNameByConnectionId(sessionId, connectionId);
                _hubContext.Clients.Group(sessionId.ToString())
                    .SendAsync(ChatEventNames.RoomMessage, $"{user} has left the room");
                _doodleChamp.RemoveFromSession(sessionId, connectionId);
            }
        }

        private void OnUpdatePlayerLeaderboard(int sessionId, List<User> users)
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
            _doodleChamp.RemoveSession(sessionId);
        }
        
        private void OnUserTurn(int sessionId, string connectionId, string username)
        {
            _hubContext.Clients.Client(connectionId)
                .SendAsync(ChatEventNames.UserTurn);

            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.RoomMessage, $"{username} is the drawer");
        }

        private void OnLookoutCondition(int sessionId, string lookoutCondition)
        {
            if (lookoutCondition == Model.Constants.DoodleChamp.OrderOfPlayList)
            {
                _doodleChamp.GenerateOrderOfPlay(sessionId);
            }
        }
    }
}
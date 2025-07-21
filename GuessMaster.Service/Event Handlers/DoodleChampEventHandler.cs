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
        }

        public void Unsubscribe()
        {
            ChatHub.UserJoinedRoom -= OnUserJoinedRoom;
            ChatHub.UserLeftRoom -= OnUserLeftRoom;
            GameTimer.TimerTick -= OnTimerTick;
            GameTimer.DCLookoutConditionAction -= OnLookoutCondition;
            Service.DoodleChamp.StartingLobbyTimer -= OnStartingLobbyTimer;
            Service.DoodleChamp.GameStarted -= OnGameStart;
        }

        private void OnUserJoinedRoom(int sessionId, int userId, string connectionId)
        {
            _doodleChamp.UpdateUserConnectionId(sessionId, userId, connectionId);

            string userName = _doodleChamp.GetUserNameById(sessionId, userId) ?? "Unknown User";
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.RoomUpdate, $"{userName} has joined the room");

            _doodleChamp.GetSessionUsers(sessionId, out List<ConnectedUser> users);
            var formattedUsers = users.Select(user => new User
            {
                Username = user.Username,
                AvatarUrl = user.AvatarUrl
            }).ToList();
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync(ChatEventNames.PlayersInSession, formattedUsers);

            _doodleChamp.CheckLobbyStatus(sessionId);
        }

        private void OnUserLeftRoom(int sessionId, string connectionId)
        {
            Console.WriteLine($"User with connection ID {connectionId} left room {sessionId}.");
            _doodleChamp.RemoveFromSession(sessionId, connectionId);
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

        private void OnLookoutCondition(int sessionId, string lookoutCondition)
        {
            if (lookoutCondition == Model.Constants.DoodleChamp.OrderOfPlayList)
            {
                _doodleChamp.GenerateOrderOfPlay(sessionId);
            }
        }
    }
}
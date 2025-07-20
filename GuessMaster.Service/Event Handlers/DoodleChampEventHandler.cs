using GuessMaster.Model.Models;
using GuessMaster.Service.Interface;
using GuessMaster.Service.Service;
using Microsoft.AspNetCore.SignalR;

namespace GuessMaster.Service.Event_Handlers
{
    public class DoodleChampEventHandler : IEventHandler
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public DoodleChampEventHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void Subscribe()
        {
            ChatHub.UserJoinedRoom += OnUserJoinedRoom;
            ChatHub.UserLeftRoom += OnUserLeftRoom;
            GameTimer.TimerTick += OnTimerTick;
            DoodleChamp.StartingLobbyTimer += OnStartingLobbyTimer;
            DoodleChamp.GameStarted += OnGameStart;
        }

        public void Unsubscribe()
        {
            ChatHub.UserJoinedRoom -= OnUserJoinedRoom;
            ChatHub.UserLeftRoom -= OnUserLeftRoom;
            GameTimer.TimerTick -= OnTimerTick;
            DoodleChamp.StartingLobbyTimer -= OnStartingLobbyTimer;
            DoodleChamp.GameStarted -= OnGameStart;
        }

        private void OnUserJoinedRoom(int sessionId, List<ConnectedUser> users)
        {
            Console.WriteLine($"User joined room {sessionId} with {users.Count} users.");
            if (users.Count >= Model.Constants.DoodleChamp.MinPlayers)
            {
                var doodleChamp = new Service.DoodleChamp(new GameTimer());
                doodleChamp.StartGame(sessionId, users);
            }
        }

        private void OnUserLeftRoom(int sessionId, string connectionId)
        {
            Console.WriteLine($"User with connection ID {connectionId} left room {sessionId}.");
        }

        private void OnTimerTick(int sessionId, int secondsLeft)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync("TimerUpdate", secondsLeft);
        }

        private void OnStartingLobbyTimer(int sessionId)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync("LobbyTimerStarted", true);
        }

        private void OnGameStart(int sessionId)
        {
            _hubContext.Clients.Group(sessionId.ToString())
                .SendAsync("GameState", true);
        }
    }
}
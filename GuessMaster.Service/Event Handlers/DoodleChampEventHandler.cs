using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuessMaster.Service.Service;
using GuessMaster.Model.Models;
using GuessMaster.Model.Constants;

namespace GuessMaster.Service.Event_Handlers
{
    public static class DoodleChampEventHandler
    {
        // Static constructor to subscribe to events
        static DoodleChampEventHandler()
        {
            ChatHub.UserJoinedRoom += OnUserJoinedRoom;
            ChatHub.UserLeftRoom += OnUserLeftRoom;
        }

        private static void OnUserJoinedRoom(int sessionId, List<ConnectedUser> users)
        {
            Console.WriteLine($"User joined room {sessionId} with {users.Count} users.");
            if (users.Count >= Model.Constants.DoodleChamp.MinPlayers)
            {
                var doodleChamp = new Service.DoodleChamp(new GameTimer());
                doodleChamp.StartGame(sessionId, users);
            }
        }

        private static void OnUserLeftRoom(int sessionId, string connectionId)
        {
            Console.WriteLine($"User with connection ID {connectionId} left room {sessionId}.");
        }
    }
}

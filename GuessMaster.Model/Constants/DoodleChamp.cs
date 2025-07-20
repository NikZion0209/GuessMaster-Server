using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Model.Constants
{
    public static class DoodleChamp
    {
        public const int GameTypeId = 1;
        public const int MaxPlayers = 8;
        public const int MinPlayers = 2;

        public const int LobbyCountdown = 20;
        public const int QuickLobbyCountdown = 5;
        public const string LobbyTimer = "Lobby Timer";
        public const int SelectionCountDown = 10;
        public const string SelectionTimer = "Selection Timer";
        public const int DrawingCountdown = 45;
        public const string DrawingTimer = "Drawing Timer";
        public const int RoundSummaryCountdown = 5;
        public const string RoundSummaryTimer = "Round Summary Timer";
    }
}

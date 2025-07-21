using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Model.Constants
{
    public static class DoodleChamp
    {
        // Game settings
        public const int GameTypeId = 1;
        public const int MaxPlayers = 8;
        public const int MinPlayers = 4;

        // State of play
        public const int PreGame = 0;
        public const int Lobby = 1;
        public const int InGame = 2;
        public const int RoundSummary = 3;
        public const int GameOver = 4;


        // Timer settings
        public const int LobbyCountdown = 20;
        public const int QuickLobbyCountdown = 10;
        public const int OrderOfPlayCountdown = 5;
        public const int SelectionCountDown = 10;
        public const int DrawingCountdown = 45;
        public const int RoundSummaryCountdown = 5;

        // Timer names
        public const string LobbyTimer = "Lobby Timer";
        public const string SelectionTimer = "Selection Timer";
        public const string DrawingTimer = "Drawing Timer";
        public const string RoundSummaryTimer = "Round Summary Timer";

        // Event names
        public const string OrderOfPlayList = "Order of Play List";
    }
}

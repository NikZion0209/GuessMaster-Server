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
        public const int MinPlayers = 2;
        public static readonly string PromptFilePath = Path.Combine(AppContext.BaseDirectory, "Constants", "WordBank.json");
        public const int DisplayedPrompts = 3;
        public const float ReleaseAmount = 0.67f; // Percentage of prompt displayed for hinting
        public const int HintCutOff = 5; // Cut-off for when hints can't be given anymore

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
        public const int DrawingCountdown = 40 + HintCutOff;
        public const int RoundSummaryCountdown = 5;
        public const int EndGameCountdown = 15;

        // Timer names
        public const string LobbyTimer = "Lobby Timer";
        public const string SelectionTimer = "Selection Timer";
        public const string DrawingTimer = "Drawing Timer";
        public const string RoundSummaryTimer = "Round Summary Timer";
        public const string EndGameTimer = "End Game Timer";

        // Event names
        public const string OrderOfPlayList = "Order of Play List";
        public const string ReleaseHint = "Release Hint";

        // Scoring
        public const int CorrectArtistScore = 10;
        public const int AllCorrectArtistScore = 50;
        public const int AllCorrectUserScore = 5;
        public const int CorrectFirstGuessScore = 50;
        public const int CorrectSecondGuessScore = 40;
        public const int CorrectThirdGuessScore = 30;
        public const int CorrectSubsequentGuessScore = 20;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Model.Constants
{
    public class ChatEventNames
    {
        public const string UserConnected = "JoinRoom";
        public const string UserDisconnected = "LeaveRoom";
        public const string RoomUpdate = "RoomUpdate";
        public const string RoomMessage = "RoomMessage";
        public const string SendMessage = "SendRoomMessage";
        public const string GetPlayersInSession = "GetPlayersInSession";
        public const string PlayersInSession = "PlayersInSession";
        public const string SendDrawing = "SendDrawing";
        public const string RecieveDrawing = "RecieveDrawing";
        public const string TimerUpdate = "TimerUpdate";
        public const string LobbyTimerStarted = "LobbyTimerStarted";
        public const string GameState = "GameState";
        public const string GameEndEarly = "GameEndEarly";
        public const string UserTurn = "UserTurn";
        public const string ReceiveBrushDrawing = "ReceiveBrushDrawing";
        public const string SendGeneratedPrompts = "SendGeneratedPrompts";
        public const string ReceiveGeneratedPrompts = "ReceiveGeneratedPrompts";
        public const string PromptSelectionEnd = "PromptSelectionEnd";
        public const string GuessingToggle = "GuessingToggle";
    }
}

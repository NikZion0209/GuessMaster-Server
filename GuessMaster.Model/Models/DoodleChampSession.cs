using GuessMaster.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Model.Models
{
    public class DoodleChampSession : GameSession
    {
        public int GameState { get; set; } = Constants.DoodleChamp.PreGame;
        public string UsersTurn { get; set; } = string.Empty;
        public int MaxPlayers { get; set; } = Constants.DoodleChamp.MaxPlayers;
        public int GameType { get; set; } = Constants.Gamemodes.DoodleChamp;
    }
}

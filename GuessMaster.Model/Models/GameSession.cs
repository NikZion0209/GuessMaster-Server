using System;
using System.Collections.Generic;

namespace GuessMaster.Data.Models;

public partial class GameSession
{
    public int SessionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PlayerCount { get; set; } = 0;
    public bool IsFull { get; set; } = false;
    public bool InPlay { get; set; } = false;
    public required int GameType { get; set; }
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

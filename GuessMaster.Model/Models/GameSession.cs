using System;
using System.Collections.Generic;

namespace GuessMaster.Data.Models;

public partial class GameSession
{
    public required int SessionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PlayerCount { get; set; }
    public bool IsFull { get; set; }
    public bool InPlay { get; set; }
    public required int GameType { get; set; }
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

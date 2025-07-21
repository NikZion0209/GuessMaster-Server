using GuessMaster.Model.Models;
using System;
using System.Collections.Generic;

namespace GuessMaster.Data.Models;

public partial class GameSession
{
    public required int SessionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int PlayerCount { get; set; } = 0;
    public bool IsFull { get; set; } = false;
    public virtual List<ConnectedUser> ConnectedUsers { get; set; } = new List<ConnectedUser>();
}

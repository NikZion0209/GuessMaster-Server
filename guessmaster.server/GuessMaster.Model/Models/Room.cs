using System;
using System.Collections.Generic;

namespace GuessMaster.Data.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public bool? IsFull { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? PlayerCount { get; set; }

    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();

    public virtual ICollection<RoomAssignment> RoomAssignments { get; set; } = new List<RoomAssignment>();
}

using System;
using System.Collections.Generic;

namespace GuessMaster.Data.Models;

public partial class GameSession
{
    public int SessionId { get; set; }

    public int? RoomId { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public virtual Room? Room { get; set; }
}

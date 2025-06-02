using System;
using System.Collections.Generic;

namespace GuessMaster.Data.Models;

public partial class RoomAssignment
{
    public int AssignmentId { get; set; }

    public int? RoomId { get; set; }

    public int? UserId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public virtual Room? Room { get; set; }

    public virtual User? User { get; set; }
}

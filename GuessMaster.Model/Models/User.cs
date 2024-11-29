using System;
using System.Collections.Generic;

namespace GuessMaster.Data.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public bool IsGuest { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public virtual ICollection<RoomAssignment> RoomAssignments { get; set; } = new List<RoomAssignment>();
}

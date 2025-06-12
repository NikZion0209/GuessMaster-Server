using System;
using System.Collections.Generic;

namespace GuessMaster.Data.Models;

public partial class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public int? SessionId { get; set; }
    public virtual GameSession? GameSession { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
}

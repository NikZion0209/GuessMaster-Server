using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuessMaster.Data.Models;

public partial class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public int? SessionId { get; set; }
    public virtual GameSession? GameSession { get; set; }
    public DateTime? CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? UpdatedAt { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
}

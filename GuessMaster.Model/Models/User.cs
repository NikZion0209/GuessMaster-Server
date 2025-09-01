using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuessMaster.Data.Models;

public partial class User
{
    public int UserId { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string AvatarId { get; set; }
    public int PremiumToken { get; set; } = 5;
    public int HighscoreDoodleChamp { get; set; } = 0;
    public int HighscoreFlagWhiz { get; set; } = 0;
    public int HighscoreWordSnap { get; set; } = 0;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

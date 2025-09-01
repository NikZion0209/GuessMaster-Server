namespace GuessMaster.Model.ViewModel;

public class RegisterUserDto
{
    public required string Username { get; set; }
    public int UserId { get; set; }
    public required string AvatarId { get; set; }
    public int PremiumTokens { get; set; }
}

public class SessionUserDto
{
    public required string Username { get; set; }
    public required string AvatarId { get; set; }
    public required int Score { get; set; }
}

public class RegistrationPostDto
{
    public string LoginName { get; set; } = string.Empty;
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string AvatarId { get; set; }
}

public class SinglePlayerSessionData
{
    public string SessionId { get; set; } = string.Empty;
    public int GameType { get; set; }
    public string UserId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime IssuedAt { get; set; }
}

public class HighScores
{
    public int HighscoreDoodleChamp { get; set; }
    public int HighscoreFlagWhiz { get; set; }
    public int HighscoreWordSnap { get; set; }
}
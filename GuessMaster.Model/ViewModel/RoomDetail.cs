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
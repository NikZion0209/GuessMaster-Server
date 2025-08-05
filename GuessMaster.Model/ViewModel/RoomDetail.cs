namespace GuessMaster.Model.ViewModel;

public class RegisterUserDto
{
    public required string Username { get; set; }
    public int UserId { get; set; }
    public required string AvatarUrl { get; set; }
}

public class SessionUserDto
{
    public required string Username { get; set; }
    public required string AvatarUrl { get; set; }
    public required int Score { get; set; }
}
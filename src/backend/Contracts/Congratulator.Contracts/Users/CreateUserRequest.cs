namespace Congratulator.Contracts.Users;

public class CreateUserRequest
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Login { get; set; }

    public required string Password { get; set; }

    public string? TelegramId { get; set; }
}

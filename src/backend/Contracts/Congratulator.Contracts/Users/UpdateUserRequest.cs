namespace Congratulator.Contracts.Users;

public class UpdateUserRequest
{
    public Guid UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? TelegramId { get; set; }
}

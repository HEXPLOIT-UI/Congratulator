namespace Congratulator.Contracts.Users;

public class UserDTO
{
    public Guid EntityId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Role { get; set; }
    public required string Login { get; set; }
    public string? TelegramId { get; set; }
}

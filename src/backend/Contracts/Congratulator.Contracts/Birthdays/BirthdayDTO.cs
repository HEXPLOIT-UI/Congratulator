namespace Congratulator.Contracts.Birthdays;

public class BirthdayDTO
{
    public Guid EntityId { get; set; }
    public Guid UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Description { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? PhotoPath { get; set; }
    public bool IsActive { get; set; } = true;
}

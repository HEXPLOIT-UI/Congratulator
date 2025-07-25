using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Congratulator.Contracts.Birthdays;

public class CreateBirthdayRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Description { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;

    [FromForm(Name = "photo")]
    public IFormFile? Photo { get; set; }
}

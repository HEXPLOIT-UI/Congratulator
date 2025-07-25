using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Congratulator.Contracts.Birthdays;

public class UpdateBirthdayRequest
{
    public Guid EntityId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Description { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool? IsActive { get; set; }

    [FromForm(Name = "photo")]
    public IFormFile? Photo { get; set; }
}

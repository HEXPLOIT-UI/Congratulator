using Congratulator.Contracts.Base;

namespace Congratulator.Contracts.Birthdays;

public class BirthdayQueryRequest : BaseRequestWithPagination
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Description { get; set; }
    public bool? ActiveOnly { get; set; }
    public bool? IncomingOnly { get; set; }
    public string? Search { get; set; } 
}
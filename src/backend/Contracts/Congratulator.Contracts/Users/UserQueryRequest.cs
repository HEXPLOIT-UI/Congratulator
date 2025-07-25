using Congratulator.Contracts.Base;

namespace Congratulator.Contracts.Users;

public class UserQueryRequest : BaseRequestWithPagination
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Login { get; set; }
    public string? Search { get; set; }
    public string? TelegramId { get; set; }
}
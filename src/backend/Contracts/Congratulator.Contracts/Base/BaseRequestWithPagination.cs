namespace Congratulator.Contracts.Base;

public class BaseRequestWithPagination
{
    public int Page { get; set; }
    public int BatchSize { get; set; }
}

namespace Congratulator.Domain.Errors;

public abstract record DomainError(string Code, string Description);
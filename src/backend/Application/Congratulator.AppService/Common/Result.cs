using Congratulator.Domain.Errors;

namespace Congratulator.AppService.Common;

public class Result
{
    protected Result(bool isSuccess, DomainError? error)
    {
        if (isSuccess && error != null ||
            !isSuccess && error == null)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public DomainError? Error { get; }

    public static Result Success()
        => new(isSuccess: true, error: null);

    public static Result Failure(DomainError error)
        => new(isSuccess: false, error);
}

public class Result<T> : Result
{
    private readonly T _value;
    private Result(T value)
        : base(isSuccess: true, error: null)
        => _value = value;

    private Result(DomainError error)
        : base(isSuccess: false, error)
    { }

    public T Value
        => IsSuccess
           ? _value
           : throw new InvalidOperationException("No 'Failure' field value");

    public static Result<T> Success(T value)
        => new(value);

    public static new Result<T> Failure(DomainError error)
        => new(error);
}
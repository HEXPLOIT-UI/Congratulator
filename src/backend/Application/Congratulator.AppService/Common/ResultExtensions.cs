using Congratulator.Domain.Errors;

namespace Congratulator.AppService.Common;

public static class ResultExtensions
{
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<DomainError, TOut> onFailure)
        => result.IsSuccess
           ? onSuccess()
           : onFailure(result.Error!);
    public static TOut Match<T, TOut>(
        this Result<T> result,
        Func<T, TOut> onSuccess,
        Func<DomainError, TOut> onFailure)
        => result.IsSuccess
           ? onSuccess(result.Value)
           : onFailure(result.Error!);
}
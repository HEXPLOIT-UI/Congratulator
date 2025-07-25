using Congratulator.Contracts.Base;
using FluentValidation;

namespace Congratulator.AppService.Validators.Commons;

public class BaseRequestWithPaginationValidator
        : AbstractValidator<BaseRequestWithPagination>
{
    public BaseRequestWithPaginationValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page должен быть >= 1");

        RuleFor(x => x.BatchSize)
            .InclusiveBetween(1, 100)
            .WithMessage("BatchSize должен быть от 1 до 100");
    }
}
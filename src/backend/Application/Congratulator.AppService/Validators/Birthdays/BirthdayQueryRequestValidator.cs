using Congratulator.AppService.Validators.Commons;
using Congratulator.Contracts.Birthdays;
using FluentValidation;

namespace Congratulator.AppService.Validators.Birthdays;

public class BirthdayQueryRequestValidator : AbstractValidator<BirthdayQueryRequest>
{
    public BirthdayQueryRequestValidator()
    {
        Include(new BaseRequestWithPaginationValidator());

        RuleFor(x => x.FirstName).Length(0, 50);
        RuleFor(x => x.LastName).Length(0, 50);
        RuleFor(x => x.Description).Length(0, 999);
        RuleFor(x => x.Search).Length(0, 255);
    }
}

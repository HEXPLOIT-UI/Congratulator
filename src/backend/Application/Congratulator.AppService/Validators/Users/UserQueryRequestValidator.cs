using Congratulator.AppService.Validators.Commons;
using Congratulator.Contracts.Users;
using FluentValidation;

namespace Congratulator.AppService.Validators.Users;

public class UserQueryRequestValidator : AbstractValidator<UserQueryRequest>
{
    public UserQueryRequestValidator()
    {
        Include(new BaseRequestWithPaginationValidator());

        RuleFor(x => x.FirstName).Length(0, 50);
        RuleFor(x => x.LastName).Length(0, 50);
        RuleFor(x => x.Login).Length(0, 64).Must(s => !s?.Any(c => !char.IsLetter(c)) ?? true);
        RuleFor(x => x.Search).Length(0, 255);
        RuleFor(x => x.TelegramId).Must(s => s?.All(c => char.IsNumber(c)) ?? true);
    }
}

using Congratulator.Contracts.Users;
using FluentValidation;

namespace Congratulator.AppService.Validators.Users;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FirstName).MaximumLength(50);
        RuleFor(x => x.LastName).MaximumLength(50);
        RuleFor(x => x.TelegramId).Must(s => s?.All(c => char.IsNumber(c)) ?? true);
    }
}

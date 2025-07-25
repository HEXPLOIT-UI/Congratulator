using Congratulator.Contracts.Users;
using FluentValidation;

namespace Congratulator.AppService.Validators.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().Length(1, 50);
        RuleFor(x => x.LastName).NotEmpty().Length(1, 50);
        RuleFor(x => x.Login).NotEmpty().Length(3, 64).Must(s => !s.Any(c => !char.IsLetter(c)));
        RuleFor(x => x.Password).NotEmpty().Length(6, 128);
        RuleFor(x => x.TelegramId).Must(s => s?.All(c => char.IsNumber(c)) ?? true);
    }
}

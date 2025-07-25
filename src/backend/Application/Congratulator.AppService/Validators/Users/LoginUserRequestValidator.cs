using Congratulator.Contracts.Users;
using FluentValidation;

namespace Congratulator.AppService.Validators.Users;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty().Length(3, 64).Must(s => !s.Any(c => !char.IsLetter(c)));
        RuleFor(x => x.Password).NotEmpty().Length(6, 128);
    }
}

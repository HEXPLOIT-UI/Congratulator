using Congratulator.Contracts.Birthdays;
using FluentValidation;

namespace Congratulator.AppService.Validators.Birthdays;

public class CreateBirthdayRequestValidator : AbstractValidator<CreateBirthdayRequest>
{
    public CreateBirthdayRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().Length(1, 50);
        RuleFor(x => x.LastName).NotEmpty().Length(1, 50);
        RuleFor(x => x.Description).Length(0, 999);
        RuleFor(x => x.DateOfBirth).NotNull();
    }
}

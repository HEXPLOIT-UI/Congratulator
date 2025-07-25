using Congratulator.Contracts.Birthdays;
using FluentValidation;

namespace Congratulator.AppService.Validators.Birthdays;

public class UpdateBirthdayRequestValidator : AbstractValidator<UpdateBirthdayRequest>
{
    public UpdateBirthdayRequestValidator()
    {
        RuleFor(x => x.EntityId).NotNull();
        RuleFor(x => x.FirstName).MaximumLength(50);
        RuleFor(x => x.LastName).MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(999);
    }
}

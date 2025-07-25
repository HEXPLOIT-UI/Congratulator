using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Congratulator.API.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _provider;

    public ValidationFilter(IServiceProvider provider)
        => _provider = provider;

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var (argName, argValue) in context.ActionArguments)
        {
            if (argValue == null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argValue.GetType());

            if (_provider.GetService(validatorType) is not IValidator validator) continue;

            var validationContext = new ValidationContext<object>(argValue);
            var result = await validator.ValidateAsync(validationContext);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );

                context.Result = new BadRequestObjectResult(
                    new ValidationProblemDetails(errors)
                );
                return;
            }
        }

        await next();
    }
}
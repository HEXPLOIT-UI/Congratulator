using Congratulator.AppService.Base;
using Congratulator.AppService.Birthdays.Services;
using Congratulator.AppService.Users.Services;
using Congratulator.Infrastructure.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Congratulator.Infrastructure.Exstensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContextPool<ApplicationDbContext>(options =>
            options.UseNpgsql(
                config.GetConnectionString("PostgreConnection"),
                opt => opt.MigrationsAssembly(typeof(ApplicationDbContext).Assembly))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
            poolSize: 1024);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBirthdayService, BirthdayService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}

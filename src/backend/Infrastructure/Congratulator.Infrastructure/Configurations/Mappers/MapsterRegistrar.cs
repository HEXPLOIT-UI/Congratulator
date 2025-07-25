using Congratulator.AppService.Utils;
using Congratulator.Contracts.Birthdays;
using Congratulator.Contracts.Users;
using Congratulator.Domain.Birthdays.Entity;
using Congratulator.Domain.Users.Entity;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Congratulator.Infrastructure.Configurations.Mappers;

public static class MapsterRegistrar
{
    public static IServiceCollection ConfigureMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.NewConfig<UserEntity, UserDTO>();
        config.NewConfig<UpdateUserRequest, UserEntity>().IgnoreNullValues(true);
        config.NewConfig<CreateUserRequest, UserEntity>()
            .Map(dest => dest.EntityId, src => Guid.NewGuid())
            .Map(dest => dest.PasswordHash, src => HashUtils.HashPassword(src.Password))
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Login, src => src.Login);

        config.NewConfig<CreateBirthdayRequest, BirthdayEntity>();
        config.NewConfig<UpdateBirthdayRequest, BirthdayEntity>().IgnoreNullValues(true);
        config.NewConfig<BirthdayEntity, BirthdayDTO>();

        config.RequireExplicitMapping = true;
        config.Compile();

        services
            .AddSingleton(config)
            .AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
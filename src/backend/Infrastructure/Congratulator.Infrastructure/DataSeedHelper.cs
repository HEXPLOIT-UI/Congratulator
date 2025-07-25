using Congratulator.Domain.Users.Entity;

namespace Congratulator.Infrastructure;

public static class DataSeedHelper
{
    static readonly Guid AdminUserId = new("bc3b8c2d-0368-4dbc-a7b2-c03c020ddcbd");
    public static UserEntity[] GetUserEntities()
    {
        return [new UserEntity {
            EntityId = AdminUserId,
            FirstName = "Admin",
            LastName = "Admin",
            Login = "admin",
            Role = "Admin",
            PasswordHash = "$2a$11$oIzawOjBdsMLJV3b0p82yOtCGDNZeRpF687i3TWDqmkLb0MrqRCY."
        }];
    }
}

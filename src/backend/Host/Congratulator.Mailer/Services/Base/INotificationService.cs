using Congratulator.Domain.Users.Entity;

namespace Congratulator.Mailer.Services.Base;

public interface INotificationService
{
    Task SendNotificationAsync(UserEntity user, string message, CancellationToken ct = default);
}
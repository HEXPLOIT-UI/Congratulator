using Congratulator.Domain.Base.Entity;
using Congratulator.Domain.Birthdays.Entity;

namespace Congratulator.Domain.Users.Entity;

/// <summary>
/// Представляет пользователя.
/// Наследует от <see cref="BaseEntity"/>.
/// </summary>
/// <seealso cref="BaseEntity"/>
public class UserEntity : BaseEntity
{
    /// <summary>
    /// Имя.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Логин авторизации.
    /// </summary>
    public required string Login { get; set; }

    /// <summary>
    /// Уровень доступа пользователя
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Хеш пароля пользователя.
    /// </summary>
    public required string PasswordHash { get; set; }

    // В задании нужно подключить лишь один канал оповещения пользователя о наступающих днях рождения, 
    // поэтому ограничимся одним выбранным мною источником и захардкодим его тут, в общем можно сделать 
    // очередное навигационное поле для хранения множества сервисов выбранных пользователем для оповещения 
    public string? TelegramId { get; set; } 

    /// <summary>
    /// Навигационное свойство - все ДР, которые “принадлежат” этому аккаунту
    /// </summary>
    public ICollection<BirthdayEntity> Birthdays { get; set; } = [];
}

using Congratulator.Domain.Base.Entity;
using Congratulator.Domain.Users.Entity;

namespace Congratulator.Domain.Birthdays.Entity;

/// <summary>
/// Представляет запись о дне рождения человека.
/// Наследует от <see cref="BaseEntity"/>.
/// </summary>
/// <seealso cref="BaseEntity"/>
public class BirthdayEntity : BaseEntity
{
    /// <summary>
    /// Идентификатор пользователя, который владеет записью о дне рождения.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Связанный объект пользователя, представляющий информацию о пользователе.
    /// </summary>
    public UserEntity? User { get; set; }

    /// <summary>
    /// Имя человека.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Фамилия человека.
    /// </summary>
    public required string LastName { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Дата рождения человека.
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Относительный путь к фотографии человека.
    /// </summary>
    public string? PhotoPath { get; set; }

    /// <summary>
    /// Указывает, активна ли запись о дне рождения.
    /// По умолчанию значение равно <c>true</c>.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

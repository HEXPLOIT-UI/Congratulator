namespace Congratulator.Domain.Base.Entity;

/// <summary>
/// Базовый класс для всех сущностей.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Идентификатор записи.
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Дата создания записи.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

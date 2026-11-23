using HosteraBackend.API.Shared.Domain.Model;

namespace HosteraBackend.API.Communications.Domain.Model.Aggregates;

/// <summary>
///     Audit extension for BookingConfirmation aggregate.
/// </summary>
/// <remarks>
///     CreatedAt and UpdatedAt are automatically managed by the persistence layer
///     via the AuditableEntityInterceptor in the Shared bounded context.
/// </remarks>
public partial class BookingConfirmation : IAuditableEntity
{
    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}

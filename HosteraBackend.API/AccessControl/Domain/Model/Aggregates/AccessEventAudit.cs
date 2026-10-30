using HosteraBackend.API.Shared.Domain.Model;

namespace HosteraBackend.API.AccessControl.Domain.Model.Aggregates;

/// <summary>
///     Audit extension for AccessEvent aggregate.
/// </summary>
/// <remarks>
///     CreatedAt and UpdatedAt are automatically managed by the persistence layer
///     via the AuditableEntityInterceptor in the Shared bounded context.
/// </remarks>
public partial class AccessEvent : IAuditableEntity
{
    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}

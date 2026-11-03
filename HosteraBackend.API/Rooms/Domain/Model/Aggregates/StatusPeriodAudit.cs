using HosteraBackend.API.Shared.Domain.Model;

namespace HosteraBackend.API.Rooms.Domain.Model.Aggregates;

public partial class StatusPeriod : IAuditableEntity
{
    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}

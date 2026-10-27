using HosteraBackend.API.Shared.Domain.Model;

namespace HosteraBackend.API.Rooms.Domain.Model.Aggregates;

public partial class DailyRate : IAuditableEntity
{
    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}

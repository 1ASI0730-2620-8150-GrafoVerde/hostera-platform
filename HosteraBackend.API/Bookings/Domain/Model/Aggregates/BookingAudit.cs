using HosteraBackend.API.Shared.Domain.Model;

namespace HosteraBackend.API.Bookings.Domain.Model.Aggregates;

public partial class Booking : IAuditableEntity
{
    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}

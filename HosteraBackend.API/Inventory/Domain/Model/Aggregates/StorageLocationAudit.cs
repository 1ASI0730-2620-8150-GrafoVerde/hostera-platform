using HosteraBackend.API.Shared.Domain.Model;

namespace HosteraBackend.API.Inventory.Domain.Model.Aggregates;

public partial class StorageLocation : IAuditableEntity
{
    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}

using HosteraBackend.API.Shared.Domain.Model;

namespace HosteraBackend.API.Iam.Domain.Model.Aggregates;

public partial class User : IAuditableEntity
{
    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}

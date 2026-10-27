namespace HosteraBackend.API.Rooms.Domain.Model.Aggregates;

/// <summary>
///     Represents a hotel property of an organization.
/// </summary>
/// <remarks>
///     Properties are read-only for now: they are seeded, because registering a hotel operation depends on the
///     identity stories planned for a later sprint.
/// </remarks>
public class Property
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Property()
    {
        Name = null!;
        City = null!;
        Currency = null!;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this property.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the organization that owns the property.
    /// </summary>
    public int OrganizationId { get; private set; }

    /// <summary>
    ///     Gets the name of the property.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the city of the property.
    /// </summary>
    public string City { get; private set; }

    /// <summary>
    ///     Gets the three-letter code of the currency of the property.
    /// </summary>
    public string Currency { get; private set; }
}

namespace HosteraBackend.API.AccessControl.Domain.Model.Aggregates;

/// <summary>
///     Represents a member of the staff of a property who can hold a staff credential.
/// </summary>
public class StaffMember
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected StaffMember()
    {
        Name = null!;
        Role = null!;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this staff member.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property where the staff member works.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the name of the staff member.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the role of the staff member.
    /// </summary>
    public string Role { get; private set; }
}

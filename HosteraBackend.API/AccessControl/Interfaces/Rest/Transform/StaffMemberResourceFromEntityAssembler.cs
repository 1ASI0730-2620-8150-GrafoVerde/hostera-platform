using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="StaffMember" /> aggregate into a
///     <see cref="StaffMemberResource" /> for REST representation.
/// </summary>
public static class StaffMemberResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="StaffMember" /> aggregate to its <see cref="StaffMemberResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="StaffMember" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="StaffMemberResource" /> representing the staff member.</returns>
    public static StaffMemberResource ToResourceFromEntity(StaffMember entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new StaffMemberResource(entity.Id, entity.PropertyId, entity.Name, entity.Role);
    }
}

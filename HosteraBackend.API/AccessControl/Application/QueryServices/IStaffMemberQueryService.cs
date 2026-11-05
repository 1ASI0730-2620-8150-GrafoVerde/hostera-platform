using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;

namespace HosteraBackend.API.AccessControl.Application.QueryServices;

/// <summary>
///     Represents the staff member query service of the Access Control bounded context.
/// </summary>
public interface IStaffMemberQueryService
{
    /// <summary>
    ///     Handles the get all staff members by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllStaffMembersByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The staff members of the property.</returns>
    Task<IEnumerable<StaffMember>> Handle(GetAllStaffMembersByPropertyIdQuery query,
        CancellationToken cancellationToken);
}

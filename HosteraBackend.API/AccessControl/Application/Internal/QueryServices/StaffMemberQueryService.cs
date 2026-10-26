using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Domain.Repositories;

namespace HosteraBackend.API.AccessControl.Application.Internal.QueryServices;

/// <summary>
///     Represents the staff member query service of the Access Control bounded context.
/// </summary>
/// <param name="staffMemberRepository">
///     The <see cref="IStaffMemberRepository" /> to use.
/// </param>
public class StaffMemberQueryService(IStaffMemberRepository staffMemberRepository) : IStaffMemberQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<StaffMember>> Handle(GetAllStaffMembersByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await staffMemberRepository.FindByPropertyIdAsync(query.PropertyId, cancellationToken);
    }
}

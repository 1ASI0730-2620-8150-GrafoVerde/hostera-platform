using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.AccessControl.Domain.Repositories;

/// <summary>
///     The staff member repository contract.
/// </summary>
public interface IStaffMemberRepository : IBaseRepository<StaffMember>
{
    /// <summary>
    ///     Find the staff members of a property
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the staff members of the property, or empty otherwise.
    /// </returns>
    Task<IEnumerable<StaffMember>> FindByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
}

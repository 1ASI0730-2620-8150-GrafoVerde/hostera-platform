using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.AccessControl.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IStaffMemberRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class StaffMemberRepository(AppDbContext context)
    : BaseRepository<StaffMember>(context), IStaffMemberRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<StaffMember>> FindByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<StaffMember>()
            .Where(staffMember => staffMember.PropertyId == propertyId)
            .OrderBy(staffMember => staffMember.Name)
            .ToListAsync(cancellationToken);
    }
}

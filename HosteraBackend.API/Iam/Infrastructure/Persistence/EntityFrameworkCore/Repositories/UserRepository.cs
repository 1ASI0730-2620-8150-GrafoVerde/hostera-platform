using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IUserRepository" />.
/// </summary>
/// <remarks>
///     Usernames are compared with the collation of the database, which ignores case in MySQL by default.
/// </remarks>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    /// <inheritdoc />
    public async Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .FirstOrDefaultAsync(user => user.Username == username, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> FindByUsernameAsNoTrackingAsync(string username,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username == username, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> FindByIdAsNoTrackingAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> ListAsNoTrackingAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .AsNoTracking()
            .OrderBy(user => user.Username)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>().AnyAsync(user => user.Username == username, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> FindLatestOrganizationIdAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>().MaxAsync(user => (int?)user.OrganizationId, cancellationToken) ?? 0;
    }
}

using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace HosteraBackend.API.Rooms.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IPropertyRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class PropertyRepository(AppDbContext context)
    : BaseRepository<Property>(context), IPropertyRepository;

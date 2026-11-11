using HosteraBackend.API.AccessControl.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using HosteraBackend.API.Bookings.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using HosteraBackend.API.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using HosteraBackend.API.Rooms.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;
namespace HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Inventory Bounded Context
        builder.ApplyInventoryConfiguration();

        // Access Control Bounded Context
        builder.ApplyAccessControlConfiguration();

        // Rooms Bounded Context
        builder.ApplyRoomsConfiguration();

        // Bookings Bounded Context
        builder.ApplyBookingsConfiguration();

        builder.UseSnakeCaseNamingConvention();
    }
}
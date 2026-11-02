using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Interceptors;
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
        
        builder.UseSnakeCaseNamingConvention();
    }
}
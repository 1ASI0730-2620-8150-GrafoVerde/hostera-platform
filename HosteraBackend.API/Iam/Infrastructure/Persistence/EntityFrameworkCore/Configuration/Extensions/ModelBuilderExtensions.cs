using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Entity Framework Core mapping of the IAM bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies the mapping of users.
    /// </summary>
    /// <param name="builder">The model builder of the application context.</param>
    public static void ApplyIamConfiguration(this ModelBuilder builder)
    {
        builder.Entity<User>(user =>
        {
            user.HasKey(u => u.Id);
            user.Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
            user.Property(u => u.OrganizationId).IsRequired();
            user.Property(u => u.Username).IsRequired().HasMaxLength(User.UsernameMaxLength);
            // BCrypt hashes are 60 characters long
            user.Property(u => u.PasswordHash).IsRequired().HasMaxLength(100);
            user.Property(u => u.FullName).IsRequired().HasMaxLength(User.FullNameMaxLength);
            // The role is stored by name, such as OperationsManager
            user.Property(u => u.Role).HasConversion<string>().IsRequired().HasMaxLength(30);
            user.HasIndex(u => u.Username).IsUnique();
            user.HasIndex(u => u.OrganizationId);
        });
    }
}

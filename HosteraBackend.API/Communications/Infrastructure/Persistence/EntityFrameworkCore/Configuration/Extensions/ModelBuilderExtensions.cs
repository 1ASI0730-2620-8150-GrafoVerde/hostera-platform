using HosteraBackend.API.Communications.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Communications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Entity Framework Core mapping of the Communications bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies the mapping of booking confirmations.
    /// </summary>
    /// <param name="builder">The model builder of the application context.</param>
    public static void ApplyCommunicationsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<BookingConfirmation>(confirmation =>
        {
            confirmation.HasKey(c => c.Id);
            confirmation.Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
            confirmation.Property(c => c.PropertyId).IsRequired();
            confirmation.Property(c => c.BookingId).IsRequired();
            confirmation.Property(c => c.BookingCode).IsRequired().HasMaxLength(20);
            confirmation.Property(c => c.RecipientEmail).IsRequired().HasMaxLength(120);
            confirmation.Property(c => c.Language).HasConversion<string>().IsRequired().HasMaxLength(5);
            confirmation.Property(c => c.Status).HasConversion<string>().IsRequired().HasMaxLength(20);
            confirmation.Property(c => c.ProviderMessageId).HasMaxLength(100);
            confirmation.Property(c => c.Reason).HasMaxLength(BookingConfirmation.MaxReasonLength);
            confirmation.Property(c => c.ProcessedAt);

            confirmation.HasIndex(c => c.BookingId);
        });
    }
}

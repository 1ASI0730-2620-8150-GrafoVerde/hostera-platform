using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Bookings.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Entity Framework Core mapping of the Bookings bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies the mapping of bookings.
    /// </summary>
    /// <param name="builder">The model builder of the application context.</param>
    public static void ApplyBookingsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Booking>(booking =>
        {
            booking.HasKey(b => b.Id);
            booking.Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();
            booking.Property(b => b.PropertyId).IsRequired();
            booking.Property(b => b.Code).IsRequired().HasMaxLength(20);
            booking.Property(b => b.Status).HasConversion<string>().IsRequired().HasMaxLength(20);
            booking.Property(b => b.GuestName).IsRequired().HasMaxLength(120);
            booking.Property(b => b.GuestEmail).IsRequired().HasMaxLength(120);
            booking.Property(b => b.GuestPhone).HasMaxLength(30);
            booking.Property(b => b.PreferredLanguage).HasConversion<string>().IsRequired().HasMaxLength(5);
            // The MySQL provider reads date columns as DateTime, so the values are converted explicitly
            booking.Property(b => b.CheckInDate)
                .HasConversion(date => date.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value))
                .HasColumnType("date")
                .IsRequired();
            booking.Property(b => b.CheckOutDate)
                .HasConversion(date => date.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value))
                .HasColumnType("date")
                .IsRequired();
            booking.Property(b => b.Guests).IsRequired();
            booking.Property(b => b.RoomTypeId).IsRequired();
            booking.Property(b => b.RoomId).IsRequired();
            booking.Property(b => b.RatePlanId).IsRequired();
            booking.Property(b => b.TotalAmount).IsRequired().HasPrecision(10, 2);
            booking.Property(b => b.GuestRequest).HasMaxLength(500);
            booking.Property(b => b.CancellationReason).HasConversion<string>().HasMaxLength(30);
            booking.Property(b => b.CancellationNote).HasMaxLength(500);
            booking.Ignore(b => b.Nights);
            booking.Ignore(b => b.LastNight);
            booking.Ignore(b => b.HoldsRoom);
            booking.Ignore(b => b.IsEditable);
            booking.Ignore(b => b.CanBeConfirmed);
            booking.Ignore(b => b.CanBeCancelled);

            // Each property numbers its own bookings, so a code is unique within its property
            booking.HasIndex(b => new { b.PropertyId, b.Code }).IsUnique();
            booking.HasIndex(b => new { b.RoomId, b.CheckInDate });
        });
    }
}

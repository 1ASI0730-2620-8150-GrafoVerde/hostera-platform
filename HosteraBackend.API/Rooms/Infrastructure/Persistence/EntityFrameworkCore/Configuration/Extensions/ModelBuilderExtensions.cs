using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Rooms.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Entity Framework Core mapping of the Rooms bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies the mapping of properties, room types, rate plans and daily rates.
    /// </summary>
    /// <param name="builder">The model builder of the application context.</param>
    public static void ApplyRoomsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Property>(property =>
        {
            property.HasKey(p => p.Id);
            property.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            property.Property(p => p.OrganizationId).IsRequired();
            property.Property(p => p.Name).IsRequired().HasMaxLength(80);
            property.Property(p => p.City).IsRequired().HasMaxLength(80);
            property.Property(p => p.Currency).IsRequired().HasColumnType("char(3)");
            property.HasData(
                new { Id = 1, OrganizationId = 1, Name = "Cartagena 04", City = "Cartagena", Currency = "PEN" },
                new { Id = 2, OrganizationId = 1, Name = "Lima 01", City = "Lima", Currency = "PEN" });
        });

        builder.Entity<RoomType>(roomType =>
        {
            roomType.HasKey(r => r.Id);
            roomType.Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
            roomType.Property(r => r.PropertyId).IsRequired();
            roomType.Property(r => r.Name).IsRequired().HasMaxLength(80);
            roomType.Property(r => r.Capacity).IsRequired();
            roomType.Property(r => r.BedConfiguration).IsRequired().HasMaxLength(80);
            roomType.Property(r => r.BaseNightlyRate).IsRequired().HasPrecision(10, 2);
            roomType.Property(r => r.Status).HasConversion<string>().IsRequired().HasMaxLength(20);
            roomType.Ignore(r => r.IsActive);
            roomType.HasIndex(r => new { r.PropertyId, r.Name }).IsUnique();
        });

        builder.Entity<RatePlan>(ratePlan =>
        {
            ratePlan.HasKey(r => r.Id);
            ratePlan.Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
            ratePlan.Property(r => r.PropertyId).IsRequired();
            ratePlan.Property(r => r.Name).IsRequired().HasMaxLength(80);
            ratePlan.Property(r => r.Refundable).IsRequired();
            ratePlan.Property(r => r.CancellationPolicy).HasColumnType("text");
            ratePlan.Property(r => r.Status).HasConversion<string>().IsRequired().HasMaxLength(20);
            ratePlan.Ignore(r => r.RoomTypeIds);
            ratePlan.Ignore(r => r.IncludedServices);
            ratePlan.HasIndex(r => new { r.PropertyId, r.Name }).IsUnique();

            ratePlan.HasMany(r => r.RoomTypes)
                .WithOne()
                .HasForeignKey(roomType => roomType.RatePlanId)
                .OnDelete(DeleteBehavior.Cascade);
            ratePlan.Navigation(r => r.RoomTypes).HasField("_roomTypes").AutoInclude();

            ratePlan.HasMany(r => r.Services)
                .WithOne()
                .HasForeignKey(service => service.RatePlanId)
                .OnDelete(DeleteBehavior.Cascade);
            ratePlan.Navigation(r => r.Services).HasField("_services").AutoInclude();
        });

        builder.Entity<RatePlanRoomType>(roomType =>
        {
            roomType.HasKey(r => new { r.RatePlanId, r.RoomTypeId });
        });

        builder.Entity<RatePlanService>(service =>
        {
            service.HasKey(s => new { s.RatePlanId, s.Service });
            service.Property(s => s.Service).HasConversion<string>().IsRequired().HasMaxLength(30);
        });

        builder.Entity<DailyRate>(dailyRate =>
        {
            dailyRate.HasKey(d => d.Id);
            dailyRate.Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();
            dailyRate.Property(d => d.PropertyId).IsRequired();
            dailyRate.Property(d => d.RoomTypeId).IsRequired();
            // The MySQL provider reads date columns as DateTime, so the value is converted explicitly
            dailyRate.Property(d => d.Date)
                .HasConversion(date => date.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value))
                .HasColumnName("rate_date")
                .HasColumnType("date")
                .IsRequired();
            dailyRate.Property(d => d.Amount).IsRequired().HasPrecision(10, 2);
            dailyRate.HasIndex(d => new { d.RatePlanId, d.RoomTypeId, d.Date }).IsUnique();

            dailyRate.HasOne<RatePlan>()
                .WithMany()
                .HasForeignKey(d => d.RatePlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

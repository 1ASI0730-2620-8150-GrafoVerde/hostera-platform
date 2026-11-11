using HosteraBackend.API.Inventory.Domain.Model.Aggregates;
using HosteraBackend.API.Inventory.Domain.Model.Entities;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Entity Framework Core mapping of the Inventory bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies the mapping of storage locations, inventory items, their stock and their adjustments.
    /// </summary>
    /// <param name="builder">The model builder of the application context.</param>
    public static void ApplyInventoryConfiguration(this ModelBuilder builder)
    {
        builder.Entity<StorageLocation>(location =>
        {
            location.HasKey(l => l.Id);
            location.Property(l => l.Id).IsRequired().ValueGeneratedOnAdd();
            location.Property(l => l.PropertyId).IsRequired();
            location.Property(l => l.Code)
                .HasConversion(code => code.Value, value => new InventoryCode(value))
                .IsRequired()
                .HasMaxLength(20);
            location.Property(l => l.Name).IsRequired().HasMaxLength(80);
            location.Property(l => l.Type).HasConversion<string>().IsRequired().HasMaxLength(20);
            location.Property(l => l.Area).IsRequired().HasMaxLength(80);
            location.Property(l => l.ResponsibleTeam).IsRequired().HasMaxLength(80);
            location.Property(l => l.Description).HasMaxLength(240);
            location.HasIndex(l => new { l.PropertyId, l.Code }).IsUnique();
        });

        builder.Entity<InventoryItem>(item =>
        {
            item.HasKey(i => i.Id);
            item.Property(i => i.Id).IsRequired().ValueGeneratedOnAdd();
            item.Property(i => i.PropertyId).IsRequired();
            item.Property(i => i.Code)
                .HasConversion(code => code.Value, value => new InventoryCode(value))
                .IsRequired()
                .HasMaxLength(20);
            item.Property(i => i.Name).IsRequired().HasMaxLength(80);
            item.Property(i => i.Category).HasConversion<string>().IsRequired().HasMaxLength(30);
            item.Property(i => i.Unit).HasConversion<string>().IsRequired().HasMaxLength(20);
            item.Property(i => i.PrimaryLocationId).IsRequired();
            item.Property(i => i.LowStockThreshold).IsRequired().HasPrecision(12, 3);
            item.Ignore(i => i.TotalQuantity);
            item.Ignore(i => i.StockCondition);
            item.HasIndex(i => new { i.PropertyId, i.Code }).IsUnique();

            item.HasMany(i => i.Stocks)
                .WithOne()
                .HasForeignKey(stock => stock.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);
            item.Navigation(i => i.Stocks).HasField("_stocks").AutoInclude();

            item.HasMany(i => i.Adjustments)
                .WithOne()
                .HasForeignKey(adjustment => adjustment.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);
            item.Navigation(i => i.Adjustments).HasField("_adjustments").AutoInclude();
        });

        builder.Entity<InventoryStock>(stock =>
        {
            stock.HasKey(s => s.Id);
            stock.Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
            stock.Property(s => s.StorageLocationId).IsRequired();
            stock.Property(s => s.Quantity).IsRequired().HasPrecision(12, 3);
            stock.HasIndex(s => new { s.InventoryItemId, s.StorageLocationId }).IsUnique();
        });

        builder.Entity<StockAdjustment>(adjustment =>
        {
            adjustment.HasKey(a => a.Id);
            adjustment.Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
            adjustment.Property(a => a.Operation).HasConversion<string>().IsRequired().HasMaxLength(20);
            adjustment.Property(a => a.QuantityChange).IsRequired().HasPrecision(12, 3);
            adjustment.Property(a => a.StorageLocationId).IsRequired();
            adjustment.Property(a => a.Reason).HasConversion<string>().IsRequired().HasMaxLength(30);
            adjustment.Property(a => a.Note).HasMaxLength(240);
            adjustment.Property(a => a.Operator).IsRequired().HasMaxLength(80);
            adjustment.Property(a => a.RecordedAt).IsRequired();
            adjustment.Property(a => a.TransferId);
        });
    }
}

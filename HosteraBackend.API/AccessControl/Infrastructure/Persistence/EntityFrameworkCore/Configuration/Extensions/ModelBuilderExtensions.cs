using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.AccessControl.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
///     Entity Framework Core mapping of the Access Control bounded context.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies the mapping of credentials, staff members and access events.
    /// </summary>
    /// <param name="builder">The model builder of the application context.</param>
    public static void ApplyAccessControlConfiguration(this ModelBuilder builder)
    {
        builder.Entity<StaffMember>(staffMember =>
        {
            staffMember.HasKey(s => s.Id);
            staffMember.Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
            staffMember.Property(s => s.PropertyId).IsRequired();
            staffMember.Property(s => s.Name).IsRequired().HasMaxLength(80);
            staffMember.Property(s => s.Role).IsRequired().HasMaxLength(80);
            staffMember.HasData(
                new { Id = 1, PropertyId = 1, Name = "Lucía Martín", Role = "Operations manager" },
                new { Id = 2, PropertyId = 1, Name = "Andrés León", Role = "Maintenance technician" },
                new { Id = 3, PropertyId = 1, Name = "Rosa Pérez", Role = "Housekeeping supervisor" },
                new { Id = 4, PropertyId = 1, Name = "Jorge Salas", Role = "Front desk agent" },
                new { Id = 5, PropertyId = 1, Name = "Diana Flores", Role = "Night auditor" },
                new { Id = 6, PropertyId = 1, Name = "Pedro Rivas", Role = "Housekeeping attendant" },
                new { Id = 7, PropertyId = 2, Name = "Carla Núñez", Role = "Front desk agent" },
                new { Id = 8, PropertyId = 2, Name = "Miguel Ortiz", Role = "Housekeeping attendant" });
        });

        builder.Entity<Credential>(credential =>
        {
            credential.HasKey(c => c.Id);
            credential.Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
            credential.Property(c => c.PropertyId).IsRequired();
            credential.Property(c => c.CardId)
                .HasConversion(cardId => cardId.Value, value => new CardId(value))
                .IsRequired()
                .HasMaxLength(4);
            credential.Property(c => c.Type).HasConversion<string>().IsRequired().HasMaxLength(20);
            credential.Property(c => c.HolderName).IsRequired().HasMaxLength(80);
            credential.Property(c => c.BookingCode).HasMaxLength(20);
            credential.Property(c => c.Scope).HasConversion<string>().HasMaxLength(20);
            credential.Property(c => c.IssuedAt).IsRequired();
            credential.Property(c => c.IssuedBy).IsRequired();
            credential.HasIndex(c => new { c.PropertyId, c.CardId });

            credential.HasOne<StaffMember>()
                .WithMany()
                .HasForeignKey(c => c.StaffMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            credential.OwnsOne(c => c.ValidityPeriod, period =>
            {
                period.WithOwner().HasForeignKey("Id");
                // Microseconds are kept, so a card ended in the same second it started still ends after it starts
                period.Property(p => p.ValidFrom).HasColumnName("valid_from").HasPrecision(6).IsRequired();
                period.Property(p => p.ValidUntil).HasColumnName("valid_until").HasPrecision(6);
            });
            credential.Navigation(c => c.ValidityPeriod).IsRequired();

            credential.OwnsOne(c => c.Revocation, revocation =>
            {
                revocation.WithOwner().HasForeignKey("Id");
                revocation.Property(r => r.Reason)
                    .HasConversion<string>()
                    .HasColumnName("revocation_reason")
                    .IsRequired()
                    .HasMaxLength(20);
                revocation.Property(r => r.Note).HasColumnName("revocation_note").IsRequired().HasMaxLength(240);
                revocation.Property(r => r.At).HasColumnName("revoked_at").IsRequired();
                revocation.Property(r => r.OperatorId).HasColumnName("revoked_by").IsRequired();
            });
        });

        builder.Entity<AccessEvent>(accessEvent =>
        {
            accessEvent.HasKey(e => e.Id);
            accessEvent.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
            accessEvent.Property(e => e.PropertyId).IsRequired();
            accessEvent.Property(e => e.CardId)
                .HasConversion(cardId => cardId.Value, value => new CardId(value))
                .IsRequired()
                .HasMaxLength(4);
            accessEvent.Property(e => e.HolderType).HasConversion<string>().IsRequired().HasMaxLength(10);
            accessEvent.Property(e => e.HolderName).IsRequired().HasMaxLength(80);
            accessEvent.Property(e => e.AccessPoint).HasConversion<string>().IsRequired().HasMaxLength(20);
            accessEvent.Property(e => e.Result).HasConversion<string>().IsRequired().HasMaxLength(10);
            accessEvent.Property(e => e.DenialReason).HasConversion<string>().HasMaxLength(20);
            accessEvent.Property(e => e.OccurredAt).IsRequired();
            accessEvent.HasIndex(e => new { e.PropertyId, e.OccurredAt });

            accessEvent.HasOne<Credential>()
                .WithMany()
                .HasForeignKey(e => e.CredentialId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

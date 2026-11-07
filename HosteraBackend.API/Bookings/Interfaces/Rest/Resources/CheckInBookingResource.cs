using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

/// <summary>
///     Check-in resource for REST API
/// </summary>
/// <param name="DocumentType">The identity document presented: dni, passport or foreign-resident-card</param>
/// <param name="DocumentNumber">The number of the identity document</param>
/// <param name="DocumentVerified">Whether reception checked that the document belongs to the guest present</param>
/// <param name="KeyCardIds">The four hexadecimal characters of each card encoded for the guest</param>
/// <param name="OperatorId">The identifier of the operator who completes the check-in</param>
public record CheckInBookingResource(
    [Required] string DocumentType,
    [Required] [MaxLength(30)] string DocumentNumber,
    bool DocumentVerified,
    [Required] IReadOnlyList<string> KeyCardIds,
    [Range(1, int.MaxValue)] int OperatorId);

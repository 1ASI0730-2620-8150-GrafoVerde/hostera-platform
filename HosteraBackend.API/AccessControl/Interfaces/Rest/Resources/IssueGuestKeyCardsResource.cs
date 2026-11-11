using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

/// <summary>
///     Issue guest key cards resource for REST API
/// </summary>
/// <param name="BookingId">The identifier of the booking</param>
/// <param name="BookingCode">The code of the booking</param>
/// <param name="RoomId">The identifier of the room the cards open</param>
/// <param name="HolderName">The name of the guest who holds the cards</param>
/// <param name="ValidFrom">The moment the access starts</param>
/// <param name="ValidUntil">The moment the access ends</param>
/// <param name="CardIds">The identifier of each encoded card</param>
/// <param name="IssuedBy">The identifier of the operator who issues the cards</param>
public record IssueGuestKeyCardsResource(
    [Range(1, int.MaxValue)] int BookingId,
    [Required] [MaxLength(20)] string BookingCode,
    [Range(1, int.MaxValue)] int RoomId,
    [Required] [MaxLength(80)] string HolderName,
    [Required] DateTimeOffset? ValidFrom,
    [Required] DateTimeOffset? ValidUntil,
    [Required] [MinLength(1)] IReadOnlyList<string> CardIds,
    [Range(1, int.MaxValue)] int IssuedBy);

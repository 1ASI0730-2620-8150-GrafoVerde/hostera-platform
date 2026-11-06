using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Bookings.Domain.Model.Commands;

/// <summary>
///     Command to start the stay of a confirmed booking and register the key cards encoded for its guest
/// </summary>
/// <param name="BookingId">The identifier of the booking</param>
/// <param name="DocumentType">The EDocumentType of the original identity document presented by the guest</param>
/// <param name="DocumentNumber">The number of the identity document</param>
/// <param name="DocumentVerified">Whether reception checked that the document belongs to the guest present</param>
/// <param name="KeyCardIds">The four hexadecimal characters of each card encoded for the guest</param>
/// <param name="OperatorId">The identifier of the operator who completes the check-in</param>
public record CheckInBookingCommand(
    int BookingId,
    EDocumentType DocumentType,
    string DocumentNumber,
    bool DocumentVerified,
    IReadOnlyList<string> KeyCardIds,
    int OperatorId);

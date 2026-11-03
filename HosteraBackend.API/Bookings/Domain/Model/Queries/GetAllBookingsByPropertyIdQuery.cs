using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Bookings.Domain.Model.Queries;

/// <summary>
///     Query to get the bookings of a property that satisfy every given criterion
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
/// <param name="Status">Keep only the bookings with this EBookingStatus, or null for all</param>
/// <param name="RoomId">Keep only the bookings of this room, or null for all</param>
/// <param name="From">Keep only the stays with a night on or after this day, or null for no lower limit</param>
/// <param name="To">Keep only the stays with a night on or before this day, or null for no upper limit</param>
/// <param name="Search">Keep only the bookings whose guest name or code contains this text, or null</param>
/// <param name="Page">The page to return, starting at 1, or null to return every matching booking</param>
/// <param name="PageSize">The number of bookings per page when a page is requested</param>
public record GetAllBookingsByPropertyIdQuery(
    int PropertyId,
    EBookingStatus? Status = null,
    int? RoomId = null,
    DateOnly? From = null,
    DateOnly? To = null,
    string? Search = null,
    int? Page = null,
    int PageSize = 50);

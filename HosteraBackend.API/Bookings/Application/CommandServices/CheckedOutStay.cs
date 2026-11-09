using HosteraBackend.API.Bookings.Domain.Model.Aggregates;

namespace HosteraBackend.API.Bookings.Application.CommandServices;

/// <summary>
///     A stay that ended: the checked-out booking, its ended key cards and whether the room was marked for cleaning.
/// </summary>
/// <param name="Booking">The checked-out booking.</param>
/// <param name="EndedKeyCards">The number of key cards that stopped opening the room.</param>
/// <param name="RoomMarkedForCleaning">
///     Whether the room was marked as Needs cleaning on the departure day. It is not marked when that day already has
///     an operational status or another guest arrives that day.
/// </param>
public record CheckedOutStay(Booking Booking, int EndedKeyCards, bool RoomMarkedForCleaning);

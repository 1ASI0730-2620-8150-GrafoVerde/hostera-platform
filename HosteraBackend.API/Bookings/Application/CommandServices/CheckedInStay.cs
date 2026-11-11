using HosteraBackend.API.Bookings.Domain.Model.Aggregates;

namespace HosteraBackend.API.Bookings.Application.CommandServices;

/// <summary>
///     A stay that started: the checked-in booking and the key cards registered for its guest.
/// </summary>
/// <param name="Booking">The checked-in booking.</param>
/// <param name="KeyCardIds">The identifiers of the key cards issued by Access Control.</param>
public record CheckedInStay(Booking Booking, IReadOnlyList<int> KeyCardIds);

using HosteraBackend.API.Bookings.Domain.Model.Aggregates;

namespace HosteraBackend.API.Bookings.Application.QueryServices;

/// <summary>
///     The bookings that match a query, with the total number of matches across every page.
/// </summary>
/// <param name="Bookings">The bookings of the requested page, or every match when no page was requested.</param>
/// <param name="TotalCount">The number of bookings that match the criteria.</param>
public record BookingPage(IEnumerable<Booking> Bookings, int TotalCount);

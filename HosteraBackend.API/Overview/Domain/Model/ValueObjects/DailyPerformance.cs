using HosteraBackend.API.Overview.Domain.Model.ReadModels;

namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     Room revenue earned on one night of a property and how many rooms were sold for it
/// </summary>
/// <param name="Date">The day of the night</param>
/// <param name="Revenue">The room revenue of the night</param>
/// <param name="SoldRooms">The rooms sold for the night</param>
/// <param name="RoomsCount">The rooms of the property</param>
public record DailyPerformance(DateOnly Date, decimal Revenue, int SoldRooms, int RoomsCount)
{
    /// <summary>
    ///     Gets the share of the rooms sold for the night, between 0 and 1.
    /// </summary>
    public decimal OccupancyRate => RoomsCount == 0 ? 0 : Math.Round((decimal)SoldRooms / RoomsCount, 4);

    /// <summary>
    ///     Builds the performance of each night of a range from the bookings of a property.
    /// </summary>
    /// <remarks>
    ///     Only confirmed, checked-in and checked-out bookings count. The total amount of each booking is spread
    ///     evenly across its nights.
    /// </remarks>
    /// <param name="bookings">The bookings of the property that include the range.</param>
    /// <param name="from">The first night of the range.</param>
    /// <param name="to">The last night of the range, included.</param>
    /// <param name="roomsCount">The rooms of the property.</param>
    /// <returns>One entry for each night, in order.</returns>
    public static IReadOnlyList<DailyPerformance> Series(
        IEnumerable<BookingReadModel> bookings,
        DateOnly from,
        DateOnly to,
        int roomsCount)
    {
        var nights = new SortedDictionary<DateOnly, (decimal Revenue, int SoldRooms)>();
        for (var night = from; night <= to; night = night.AddDays(1))
            nights[night] = (0, 0);

        foreach (var booking in bookings.Where(booking => booking.EarnsRevenue))
        {
            var bookingNights = booking.Nights.ToList();
            if (bookingNights.Count == 0) continue;

            var nightlyRevenue = booking.TotalAmount / bookingNights.Count;
            foreach (var night in bookingNights.Where(nights.ContainsKey))
            {
                var (revenue, soldRooms) = nights[night];
                nights[night] = (revenue + nightlyRevenue, soldRooms + 1);
            }
        }

        return nights
            .Select(night => new DailyPerformance(
                night.Key,
                Math.Round(night.Value.Revenue, 2),
                night.Value.SoldRooms,
                roomsCount))
            .ToList();
    }
}

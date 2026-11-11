using HosteraBackend.API.Overview.Application.Errors;
using HosteraBackend.API.Overview.Application.Internal.OutboundServices.ACL;
using HosteraBackend.API.Overview.Application.QueryServices;
using HosteraBackend.API.Overview.Domain.Model.Queries;
using HosteraBackend.API.Overview.Domain.Model.ReadModels;
using HosteraBackend.API.Overview.Domain.Model.ValueObjects;
using HosteraBackend.API.Overview.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Overview.Application.Internal.QueryServices;

/// <summary>
///     Represents the query service of the Overview bounded context.
/// </summary>
/// <param name="overviewReadRepository">
///     The <see cref="IOverviewReadRepository" /> used to read rooms, status periods and bookings.
/// </param>
/// <param name="externalPropertyService">
///     The <see cref="ExternalPropertyService" /> used to read the properties.
/// </param>
/// <param name="externalInventoryService">
///     The <see cref="ExternalInventoryService" /> used to read the stock condition.
/// </param>
/// <param name="externalAccessControlService">
///     The <see cref="ExternalAccessControlService" /> used to read the access attempts.
/// </param>
public class OverviewQueryService(
    IOverviewReadRepository overviewReadRepository,
    ExternalPropertyService externalPropertyService,
    ExternalInventoryService externalInventoryService,
    ExternalAccessControlService externalAccessControlService)
    : IOverviewQueryService
{
    /// <summary>
    ///     Longest range of nights accepted for the daily performance.
    /// </summary>
    public const int MaxPerformanceNights = 92;

    /// <inheritdoc />
    public async Task<Result<OperationalDashboard, OverviewError>> Handle(GetDashboardByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.From > query.To || query.To.DayNumber - query.From.DayNumber + 1 > MaxPerformanceNights)
            return new Result<OperationalDashboard, OverviewError>.Failure(OverviewError.InvalidInput);

        var property = await externalPropertyService.FetchPropertyByIdAsync(query.PropertyId, cancellationToken);
        if (property is null)
            return new Result<OperationalDashboard, OverviewError>.Failure(OverviewError.PropertyNotFound);

        // The bookings are read once for today and for the nights of the performance range
        var firstNight = query.From < query.Date ? query.From : query.Date;
        var lastNight = query.To > query.Date ? query.To : query.Date;

        var rooms = await overviewReadRepository.FindRoomsByPropertyIdAsync(property.Id, cancellationToken);
        var statusPeriods = await overviewReadRepository.FindStatusPeriodsByPropertyIdAsync(property.Id,
            query.Date, query.Date, cancellationToken);
        var bookings = await overviewReadRepository.FindBookingsByPropertyIdAsync(property.Id, firstNight,
            lastNight, cancellationToken);
        var inventory = await externalInventoryService.FetchInventorySummaryAsync(property.Id, cancellationToken);
        var access = await externalAccessControlService.FetchAccessSummaryAsync(property.Id, query.Date,
            cancellationToken);

        var unavailableSections = new List<EDashboardSection>();
        if (rooms is null || statusPeriods is null) unavailableSections.Add(EDashboardSection.Rooms);
        if (bookings is null) unavailableSections.Add(EDashboardSection.Bookings);
        if (inventory is null) unavailableSections.Add(EDashboardSection.Inventory);
        if (access is null) unavailableSections.Add(EDashboardSection.AccessControl);

        var roomsAvailable = rooms is not null && statusPeriods is not null;
        var dashboard = new OperationalDashboard(
            property.Id,
            property.Name,
            query.Date,
            roomsAvailable && bookings is not null
                ? PropertyOverview.For(property, rooms!, bookings, statusPeriods!, query.Date)
                : null,
            roomsAvailable && bookings is not null
                ? RoomStatusSummary.For(rooms!, bookings, statusPeriods!, query.Date)
                : null,
            bookings is not null
                ? ArrivalSummary.For(bookings, rooms ?? [], query.Date)
                : null,
            bookings is not null && rooms is not null
                ? DailyPerformance.Series(bookings, query.From, query.To, rooms.Count)
                : null,
            inventory,
            access,
            unavailableSections);

        return new Result<OperationalDashboard, OverviewError>.Success(dashboard);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<PropertyOverview> Overviews, IReadOnlyList<EDashboardSection> UnavailableSections)>
        Handle(GetAllPropertyOverviewsQuery query, CancellationToken cancellationToken)
    {
        var properties = await externalPropertyService.FetchAllPropertiesAsync(cancellationToken);
        var overviews = new List<PropertyOverview>();
        var unavailableSections = new HashSet<EDashboardSection>();

        foreach (var property in properties)
        {
            var overview = await OverviewOfAsync(property, query.Date, unavailableSections, cancellationToken);
            if (overview is not null) overviews.Add(overview);
        }

        return (overviews, unavailableSections.Order().ToList());
    }

    private async Task<PropertyOverview?> OverviewOfAsync(PropertyReadModel property, DateOnly date,
        ISet<EDashboardSection> unavailableSections, CancellationToken cancellationToken)
    {
        var rooms = await overviewReadRepository.FindRoomsByPropertyIdAsync(property.Id, cancellationToken);
        var statusPeriods = await overviewReadRepository.FindStatusPeriodsByPropertyIdAsync(property.Id, date, date,
            cancellationToken);
        var bookings = await overviewReadRepository.FindBookingsByPropertyIdAsync(property.Id, date, date,
            cancellationToken);

        if (rooms is null || statusPeriods is null) unavailableSections.Add(EDashboardSection.Rooms);
        if (bookings is null) unavailableSections.Add(EDashboardSection.Bookings);

        return rooms is null || statusPeriods is null || bookings is null
            ? null
            : PropertyOverview.For(property, rooms, bookings, statusPeriods, date);
    }
}

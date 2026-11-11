using HosteraBackend.API.AccessControl.Application.CommandServices;
using HosteraBackend.API.AccessControl.Domain.Model.Commands;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Application.CommandServices;
using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Domain.Repositories;
using HosteraBackend.API.Inventory.Application.CommandServices;
using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;
using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;
using HosteraBackend.API.Shared.Application.Internal.OutboundServices;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Shared.Infrastructure.Seeding;

/// <summary>
///     Loads demonstration data into an empty development database, so the frontend shows a working hotel.
/// </summary>
/// <remarks>
///     The catalogs (room types, rooms, rate plans, storage locations, inventory items and staff credentials) follow the
///     frontend mock data. Bookings, payments, operational statuses and access events are placed around the current day,
///     so the overview always has past revenue, stays in progress, today's arrivals and future bookings. Everything goes
///     through the application services, so the data respects the same rules as the API. Stays that already ended are
///     the exception: their check-in and check-out are recorded on past days, which the API only allows on the current
///     day.
/// </remarks>
public class DemoDataSeeder(
    AppDbContext context,
    IClock clock,
    IRoomTypeCommandService roomTypeCommandService,
    IRoomCommandService roomCommandService,
    IRatePlanCommandService ratePlanCommandService,
    IStatusPeriodCommandService statusPeriodCommandService,
    IStorageLocationCommandService storageLocationCommandService,
    IInventoryItemCommandService inventoryItemCommandService,
    ICredentialCommandService credentialCommandService,
    IAccessEventCommandService accessEventCommandService,
    IBookingCommandService bookingCommandService,
    IPaymentCommandService paymentCommandService,
    IStayCommandService stayCommandService,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    ILogger<DemoDataSeeder> logger)
{
    private const int FrontDeskAgentId = 4;
    private const string DemoOperator = "Demo operator";

    private readonly Dictionary<(int PropertyId, string Name), int> _roomTypeIds = [];
    private readonly Dictionary<(int PropertyId, string Number), int> _roomIds = [];
    private readonly Dictionary<(int PropertyId, string Name), int> _ratePlanIds = [];
    private readonly Dictionary<(int PropertyId, string Code), int> _storageLocationIds = [];
    private readonly Dictionary<(int PropertyId, string Code), int> _inventoryItemIds = [];
    private int _guestCardNumber = 0xA100;

    /// <summary>
    ///     Loads the demonstration data when the database has no room types yet.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await context.Set<RoomType>().AnyAsync(cancellationToken))
        {
            logger.LogInformation("The database already has data, so the demonstration data was not loaded.");
            return;
        }

        try
        {
            await SeedRoomsAsync(cancellationToken);
            await SeedInventoryAsync(cancellationToken);
            await SeedStaffCredentialsAsync(cancellationToken);
            await SeedBookingsAsync(cancellationToken);
            await SeedStatusPeriodsAsync(cancellationToken);
            await SeedAccessEventsAsync(cancellationToken);
            logger.LogInformation("The demonstration data was loaded around {Today}.", clock.Today);
        }
        catch (InvalidOperationException exception)
        {
            logger.LogError(exception, "The demonstration data could not be loaded completely.");
        }
    }

    private async Task SeedRoomsAsync(CancellationToken cancellationToken)
    {
        (int PropertyId, string Name, int Capacity, string Beds, decimal Rate)[] roomTypes =
        [
            (1, "Standard", 2, "1 queen", 180), (1, "Twin", 2, "2 single", 200),
            (1, "Deluxe", 4, "1 king + sofa bed", 290), (1, "Suite", 4, "1 king + sofa bed", 420),
            (1, "Family", 5, "2 double + 1 single", 340), (2, "Standard", 2, "1 queen", 150),
            (2, "Deluxe", 3, "1 king + 1 single", 240)
        ];
        foreach (var (propertyId, name, capacity, beds, rate) in roomTypes)
        {
            var roomType = Expect(await roomTypeCommandService.Handle(
                new CreateRoomTypeCommand(propertyId, name, capacity, beds, rate), cancellationToken), name);
            _roomTypeIds[(propertyId, name)] = roomType.Id;
        }

        (int PropertyId, string Number, string RoomType, int Floor)[] rooms =
        [
            (1, "201", "Standard", 2), (1, "203", "Twin", 2), (1, "210", "Standard", 2), (1, "214", "Twin", 2),
            (1, "301", "Standard", 3), (1, "305", "Deluxe", 3), (1, "308", "Twin", 3), (1, "312", "Standard", 3),
            (1, "402", "Standard", 4), (1, "407", "Twin", 4), (1, "410", "Deluxe", 4), (1, "412", "Standard", 4),
            (1, "501", "Standard", 5), (1, "507", "Deluxe", 5), (1, "509", "Family", 5), (1, "601", "Suite", 6),
            (1, "608", "Suite", 6), (2, "101", "Standard", 1), (2, "102", "Standard", 1), (2, "103", "Deluxe", 1),
            (2, "201", "Standard", 2), (2, "202", "Deluxe", 2)
        ];
        foreach (var (propertyId, number, roomType, floor) in rooms)
        {
            var room = Expect(await roomCommandService.Handle(
                new CreateRoomCommand(propertyId, number, _roomTypeIds[(propertyId, roomType)], floor),
                cancellationToken), $"room {number}");
            _roomIds[(propertyId, number)] = room.Id;
        }

        // The Family type keeps its room but no longer takes new ones
        Expect(await roomTypeCommandService.Handle(new UpdateRoomTypeCommand(_roomTypeIds[(1, "Family")], "Family", 5,
            "2 double + 1 single", 340, ERecordStatus.Inactive), cancellationToken), "Family status");

        (int PropertyId, string Name, bool Refundable, string? Policy, EIncludedService Service, string[] RoomTypes)[]
            ratePlans =
            [
                (1, "Flexible", true, "Free cancellation until 24 hours before arrival", EIncludedService.RoomOnly, []),
                (1, "Breakfast included", true, "Free cancellation until 48 hours before arrival",
                    EIncludedService.BreakfastIncluded, []),
                (1, "Non-refundable", false, null, EIncludedService.RoomOnly, []),
                (1, "Honeymoon", true, "Free cancellation until 7 days before arrival", EIncludedService.HalfBoard,
                    ["Suite"]),
                (1, "Summer 2026", true, "Free cancellation until 72 hours before arrival",
                    EIncludedService.BreakfastIncluded, ["Standard", "Twin", "Deluxe"]),
                (2, "Flexible", true, "Free cancellation until 24 hours before arrival", EIncludedService.RoomOnly, [])
            ];
        foreach (var (propertyId, name, refundable, policy, service, roomTypeNames) in ratePlans)
        {
            var roomTypeIds = roomTypeNames.Select(roomType => _roomTypeIds[(propertyId, roomType)]).ToList();
            var ratePlan = Expect(await ratePlanCommandService.Handle(
                new CreateRatePlanCommand(propertyId, name, refundable, policy, [service], roomTypeIds),
                cancellationToken), name);
            _ratePlanIds[(propertyId, name)] = ratePlan.Id;
        }

        Expect(await ratePlanCommandService.Handle(new UpdateRatePlanCommand(_ratePlanIds[(1, "Summer 2026")],
            "Summer 2026", true, "Free cancellation until 72 hours before arrival",
            [EIncludedService.BreakfastIncluded],
            [_roomTypeIds[(1, "Standard")], _roomTypeIds[(1, "Twin")], _roomTypeIds[(1, "Deluxe")]],
            ERecordStatus.Inactive), cancellationToken), "Summer 2026 status");

        // Weekend nights cost more under the flexible plans
        (int PropertyId, string RoomType, decimal Rate)[] weekendRates =
            [(1, "Standard", 210), (1, "Twin", 230), (2, "Standard", 170)];
        for (var night = clock.Today.AddDays(-35); night <= clock.Today.AddDays(35); night = night.AddDays(1))
        {
            if (night.DayOfWeek is not (DayOfWeek.Friday or DayOfWeek.Saturday))
                continue;

            foreach (var (propertyId, roomType, rate) in weekendRates)
                Expect(await ratePlanCommandService.Handle(new SetDailyRatesCommand(
                    _ratePlanIds[(propertyId, "Flexible")], _roomTypeIds[(propertyId, roomType)], night, night, rate),
                    cancellationToken), $"weekend rate {night}");
        }
    }

    private async Task SeedInventoryAsync(CancellationToken cancellationToken)
    {
        (int PropertyId, string Code, string Name, EStorageLocationType Type, string Area, string Team)[] locations =
        [
            (1, "HK-01", "Housekeeping", EStorageLocationType.Warehouse, "Ground floor", "Housekeeping"),
            (1, "MAIN-01", "Main storage", EStorageLocationType.Warehouse, "Basement", "Operations"),
            (1, "F3-01", "Floor 3 closet", EStorageLocationType.Closet, "Floor 3", "Housekeeping"),
            (1, "FD-01", "Front desk", EStorageLocationType.FrontDesk, "Lobby", "Front desk"),
            (2, "MAIN-01", "Main storage", EStorageLocationType.Warehouse, "Ground floor", "Operations")
        ];
        foreach (var (propertyId, code, name, type, area, team) in locations)
        {
            var location = Expect(await storageLocationCommandService.Handle(
                new CreateStorageLocationCommand(propertyId, new InventoryCode(code), name, type, area, team, null),
                cancellationToken), name);
            _storageLocationIds[(propertyId, code)] = location.Id;
        }

        (int PropertyId, string Code, string Name, EInventoryCategory Category, EInventoryUnit Unit, string Location,
            decimal Threshold, decimal Opening)[] items =
            [
                (1, "LIN-BT-001", "Bath towels", EInventoryCategory.Linen, EInventoryUnit.Units, "HK-01", 30, 84),
                (1, "LIN-QN-002", "Queen sheet sets", EInventoryCategory.Linen, EInventoryUnit.Units, "HK-01", 20, 18),
                (1, "AM-SH-003", "Shampoo bottles", EInventoryCategory.GuestAmenities, EInventoryUnit.Units, "MAIN-01",
                    40, 135),
                (1, "AC-KC-004", "RFID key cards", EInventoryCategory.AccessSupplies, EInventoryUnit.Units, "FD-01", 20,
                    12),
                (1, "CL-AP-005", "All-purpose cleaner", EInventoryCategory.CleaningSupplies, EInventoryUnit.Liters,
                    "MAIN-01", 5, 0),
                (1, "CL-LD-006", "Laundry detergent", EInventoryCategory.CleaningSupplies, EInventoryUnit.Kilograms,
                    "HK-01", 8, 12.5m),
                (2, "LIN-BT-001", "Bath towels", EInventoryCategory.Linen, EInventoryUnit.Units, "MAIN-01", 10, 32),
                (2, "AM-HS-002", "Hand soap", EInventoryCategory.GuestAmenities, EInventoryUnit.Liters, "MAIN-01", 5,
                    3.5m)
            ];
        foreach (var (propertyId, code, name, category, unit, location, threshold, opening) in items)
        {
            var item = Expect(await inventoryItemCommandService.Handle(new CreateInventoryItemCommand(propertyId,
                new InventoryCode(code), name, category, unit, _storageLocationIds[(propertyId, location)],
                threshold, opening, DemoOperator), cancellationToken), name);
            _inventoryItemIds[(propertyId, code)] = item.Id;
        }

        (string Code, EStockOperation Operation, decimal Quantity, string From, string? To,
            EStockAdjustmentReason Reason)[] movements =
            [
                ("LIN-BT-001", EStockOperation.Transfer, 24, "HK-01", "F3-01", EStockAdjustmentReason.InternalTransfer),
                ("LIN-QN-002", EStockOperation.Transfer, 8, "HK-01", "F3-01", EStockAdjustmentReason.InternalTransfer),
                ("AM-SH-003", EStockOperation.Out, 15, "MAIN-01", null, EStockAdjustmentReason.OperationalConsumption)
            ];
        foreach (var (code, operation, quantity, from, to, reason) in movements)
            Expect(await inventoryItemCommandService.Handle(new AdjustStockCommand(_inventoryItemIds[(1, code)],
                operation, quantity, _storageLocationIds[(1, from)],
                to is null ? null : _storageLocationIds[(1, to)], reason, null, DemoOperator), cancellationToken),
                $"{code} movement");
    }

    private async Task SeedStaffCredentialsAsync(CancellationToken cancellationToken)
    {
        (int PropertyId, int StaffMemberId, EAccessScope Scope, string CardId)[] staffCredentials =
        [
            (1, 1, EAccessScope.ServiceAreas, "F47D"), (1, 2, EAccessScope.AllAreas, "1C3A"),
            (1, 3, EAccessScope.AllRooms, "9AE2"), (1, 4, EAccessScope.ServiceAreas, "5B6C"),
            (1, 6, EAccessScope.AllRooms, "C0D8"), (2, 7, EAccessScope.ServiceAreas, "7E44")
        ];
        var validFrom = clock.Now.AddDays(-90);
        foreach (var (propertyId, staffMemberId, scope, cardId) in staffCredentials)
            Expect(await credentialCommandService.Handle(new IssueStaffCredentialCommand(propertyId, staffMemberId,
                scope, new ValidityPeriod(validFrom, null), new CardId(cardId), 1), cancellationToken),
                $"staff card {cardId}");
    }

    private async Task SeedBookingsAsync(CancellationToken cancellationToken)
    {
        // Stays that already ended, which make up the revenue and occupancy of the last weeks
        (int PropertyId, string Room, string Guest, int Arrival, int Nights, string Plan)[] pastStays =
        [
            (1, "201", "Camila Vega", -28, 3, "Flexible"), (1, "201", "James Whitaker", -20, 4, "Breakfast included"),
            (1, "201", "Lucía Paredes", -9, 3, "Flexible"), (1, "203", "Diego Navarro", -26, 2, "Non-refundable"),
            (1, "203", "Elena Ferrer", -15, 5, "Flexible"), (1, "210", "Noah Bennett", -25, 4, "Flexible"),
            (1, "210", "Paula Vargas", -12, 2, "Breakfast included"), (1, "210", "Marc Roldán", -4, 2, "Flexible"),
            (1, "214", "Sofía Ramírez", -22, 3, "Flexible"), (1, "301", "Tomás Herrera", -18, 6, "Non-refundable"),
            (1, "305", "Olivia Grant", -27, 2, "Breakfast included"), (1, "305", "Rodrigo Flores", -14, 3, "Flexible"),
            (1, "308", "Isabel Mendoza", -10, 4, "Flexible"), (1, "312", "Lucas Moreno", -21, 2, "Flexible"),
            (1, "312", "Gabriela Chávez", -6, 3, "Breakfast included"), (1, "402", "Javier Castillo", -16, 3, "Flexible"),
            (1, "407", "Adriana Torres", -24, 5, "Flexible"), (1, "407", "Mateo Rojas", -8, 2, "Non-refundable"),
            (1, "410", "Carmen Huamán", -19, 4, "Breakfast included"), (1, "601", "Andrés Salazar", -13, 3, "Flexible"),
            (1, "608", "Valeria Quispe", -23, 2, "Flexible"), (1, "608", "Emily Carter", -5, 4, "Breakfast included"),
            (2, "101", "Lucía Paredes", -20, 3, "Flexible"), (2, "101", "Diego Navarro", -7, 2, "Flexible"),
            (2, "102", "Paula Vargas", -15, 4, "Flexible"), (2, "103", "James Whitaker", -10, 3, "Flexible"),
            (2, "201", "Elena Ferrer", -5, 2, "Flexible"), (2, "202", "Noah Bennett", -12, 5, "Flexible")
        ];
        foreach (var (propertyId, room, guest, arrival, nights, plan) in pastStays)
            await SeedPastStayAsync(propertyId, room, guest, arrival, nights, plan, cancellationToken);

        // Guests leaving today, who checked in on their arrival day
        (int PropertyId, string Room, string Guest, int Arrival, int Nights)[] departures =
            [(1, "203", "Sofía Ramírez", -3, 3), (2, "201", "Rodrigo Flores", -2, 2)];
        foreach (var (propertyId, room, guest, arrival, nights) in departures)
        {
            var bookingId = await CreateBookingAsync(propertyId, room, guest, arrival, nights, "Flexible",
                cancellationToken);
            await ConfirmAsync(bookingId, cancellationToken);
            var checkInDate = clock.Today.AddDays(arrival);
            await PayShareAsync(bookingId, 1, BeforeNow(clock.AtLocalTime(checkInDate, new TimeOnly(15, 30))),
                EPaymentMethod.Cash, cancellationToken);
            var booking = await bookingRepository.FindByIdAsync(bookingId, cancellationToken)
                          ?? throw new InvalidOperationException($"The booking of {guest} was not saved.");
            booking.CheckIn(checkInDate, EDocumentType.Dni, $"{40000000 + bookingId}", true,
                clock.AtLocalTime(checkInDate, new TimeOnly(15, 0)), FrontDeskAgentId);
            await unitOfWork.CompleteAsync(cancellationToken);
        }

        // Guests staying now; the last one arrived today
        (int PropertyId, string Room, string Guest, int Arrival, int Nights, decimal PaidShare)[] currentStays =
        [
            (1, "214", "Olivia Grant", -2, 4, 0.5m),
            (1, "301", "Rodrigo Flores", -1, 3, 1), (1, "410", "Isabel Mendoza", -3, 5, 0.4m),
            (2, "101", "Gabriela Chávez", -1, 3, 1), (1, "601", "James Whitaker", 0, 3, 0.5m)
        ];
        foreach (var (propertyId, room, guest, arrival, nights, paidShare) in currentStays)
        {
            var bookingId = await CreateBookingAsync(propertyId, room, guest, arrival, nights, "Flexible",
                cancellationToken);
            await ConfirmAsync(bookingId, cancellationToken);
            await PayShareAsync(bookingId, paidShare,
                BeforeNow(clock.AtLocalTime(clock.Today.AddDays(arrival), new TimeOnly(9, 0))),
                EPaymentMethod.CardTerminal, cancellationToken);
            Expect(await stayCommandService.Handle(new CheckInBookingCommand(bookingId, EDocumentType.Passport,
                $"P{bookingId:D7}", true, [NextGuestCardId()], FrontDeskAgentId), cancellationToken),
                $"check-in of {guest}");
        }

        // Today's arrivals still waiting for their guests
        var arrivalPayment = BeforeNow(clock.AtLocalTime(clock.Today, new TimeOnly(0, 5)));
        var deposit = await CreateBookingAsync(1, "305", "Camila Vega", 0, 2, "Breakfast included", cancellationToken);
        await ConfirmAsync(deposit, cancellationToken);
        await PayShareAsync(deposit, 0.3m, arrivalPayment, EPaymentMethod.BankTransfer, cancellationToken);
        await ConfirmAsync(await CreateBookingAsync(1, "312", "Marc Roldán", 0, 3, "Flexible", cancellationToken),
            cancellationToken);
        await CreateBookingAsync(1, "402", "Valeria Quispe", 0, 2, "Non-refundable", cancellationToken);
        await ConfirmAsync(await CreateBookingAsync(2, "102", "Tomás Herrera", 0, 2, "Flexible", cancellationToken),
            cancellationToken);

        // A guest who did not arrive yesterday
        var noShow = await CreateBookingAsync(1, "412", "Lucas Moreno", -1, 2, "Flexible", cancellationToken);
        await ConfirmAsync(noShow, cancellationToken);
        Expect(await bookingCommandService.Handle(new TransitionBookingCommand(noShow, EBookingStatus.NoShow,
            FrontDeskAgentId), cancellationToken), "no-show");

        // Bookings ahead, one of them cancelled
        var paidAhead = await CreateBookingAsync(1, "407", "Adriana Torres", 1, 4, "Flexible", cancellationToken);
        await ConfirmAsync(paidAhead, cancellationToken);
        await PayShareAsync(paidAhead, 1, arrivalPayment, EPaymentMethod.CardTerminal, cancellationToken);
        await ConfirmAsync(await CreateBookingAsync(1, "201", "Elena Ferrer", 2, 3, "Breakfast included",
            cancellationToken), cancellationToken);
        await ConfirmAsync(await CreateBookingAsync(1, "501", "Carmen Huamán", 3, 3, "Flexible", cancellationToken),
            cancellationToken);
        await CreateBookingAsync(1, "412", "Javier Castillo", 5, 2, "Flexible", cancellationToken);
        await ConfirmAsync(await CreateBookingAsync(1, "507", "Andrés Salazar", 7, 4, "Breakfast included",
            cancellationToken), cancellationToken);
        await ConfirmAsync(await CreateBookingAsync(1, "608", "Mateo Rojas", 10, 3, "Honeymoon", cancellationToken),
            cancellationToken);
        await CreateBookingAsync(1, "210", "Paula Vargas", 12, 2, "Flexible", cancellationToken);
        await CreateBookingAsync(2, "103", "Noah Bennett", 3, 2, "Flexible", cancellationToken);
        await ConfirmAsync(await CreateBookingAsync(2, "202", "Lucía Paredes", 1, 3, "Flexible", cancellationToken),
            cancellationToken);
        var cancelled = await CreateBookingAsync(1, "308", "Diego Navarro", 4, 2, "Flexible", cancellationToken);
        Expect(await bookingCommandService.Handle(new TransitionBookingCommand(cancelled, EBookingStatus.Cancelled,
            FrontDeskAgentId, ECancellationReason.ChangeOfPlans), cancellationToken), "cancellation");
    }

    private async Task SeedPastStayAsync(int propertyId, string room, string guest, int arrival, int nights,
        string plan, CancellationToken cancellationToken)
    {
        var bookingId = await CreateBookingAsync(propertyId, room, guest, arrival, nights, plan, cancellationToken);
        await ConfirmAsync(bookingId, cancellationToken);

        var checkInDate = clock.Today.AddDays(arrival);
        var checkOutDate = checkInDate.AddDays(nights);
        await PayShareAsync(bookingId, 1, clock.AtLocalTime(checkInDate, new TimeOnly(15, 30)),
            EPaymentMethod.CardTerminal, cancellationToken);

        var booking = await bookingRepository.FindByIdAsync(bookingId, cancellationToken)
                      ?? throw new InvalidOperationException($"The booking of {guest} was not saved.");
        booking.CheckIn(checkInDate, EDocumentType.Dni, $"{40000000 + bookingId}", true,
            clock.AtLocalTime(checkInDate, new TimeOnly(15, 0)), FrontDeskAgentId);
        booking.CheckOut(0, ERoomCondition.NoIssues, null, clock.AtLocalTime(checkOutDate, new TimeOnly(10, 30)),
            FrontDeskAgentId);
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    private async Task SeedStatusPeriodsAsync(CancellationToken cancellationToken)
    {
        (int PropertyId, string Room, EOperationalStatus Status, int From, int To, string Reason)[] statusPeriods =
        [
            (1, "507", EOperationalStatus.OutOfService, 0, 2, "Air conditioning maintenance"),
            (1, "509", EOperationalStatus.OutOfService, -5, 20, "Renovation"),
            (1, "210", EOperationalStatus.NeedsCleaning, 0, 0, "Deep cleaning after checkout"),
            (1, "608", EOperationalStatus.NeedsCleaning, 0, 0, "Deep cleaning after checkout"),
            (1, "308", EOperationalStatus.Blocked, 7, 9, "Held for a corporate group"),
            (1, "407", EOperationalStatus.Blocked, 6, 7, "Staff training"),
            (2, "201", EOperationalStatus.Blocked, 1, 3, "Owner use")
        ];
        foreach (var (propertyId, room, status, from, to, reason) in statusPeriods)
            Expect(await statusPeriodCommandService.Handle(new SetRoomStatusCommand(_roomIds[(propertyId, room)],
                status, clock.Today.AddDays(from), clock.Today.AddDays(to), reason), cancellationToken),
                $"status of room {room}");
    }

    private async Task SeedAccessEventsAsync(CancellationToken cancellationToken)
    {
        (string CardId, EAccessPoint Point, string? Room, int MinutesAgo)[] staffEvents =
        [
            ("F47D", EAccessPoint.ServiceCorridor, null, 400), ("1C3A", EAccessPoint.MainEntrance, null, 380),
            ("9AE2", EAccessPoint.RoomDoor, "305", 350), ("5B6C", EAccessPoint.StorageRoom, null, 300),
            ("C0D8", EAccessPoint.RoomDoor, "210", 240), ("F47D", EAccessPoint.RoomDoor, "601", 200),
            ("1C3A", EAccessPoint.StorageRoom, null, 150), ("9AE2", EAccessPoint.Elevator, null, 90),
            ("5B6C", EAccessPoint.MainEntrance, null, 45), ("C0D8", EAccessPoint.ServiceCorridor, null, 20)
        ];
        foreach (var (cardId, point, room, minutesAgo) in staffEvents)
            Expect(await accessEventCommandService.Handle(new RecordAccessEventCommand(1, new CardId(cardId), point,
                room is null ? null : _roomIds[(1, room)], clock.Now.AddMinutes(-minutesAgo)), cancellationToken),
                $"access of {cardId}");
    }

    private async Task<int> CreateBookingAsync(int propertyId, string room, string guest, int arrival, int nights,
        string plan, CancellationToken cancellationToken)
    {
        var roomId = _roomIds[(propertyId, room)];
        var roomTypeId = (await context.Set<Room>().FindAsync([roomId], cancellationToken))!.RoomTypeId;
        var checkInDate = clock.Today.AddDays(arrival);
        var email = $"{guest.Split(' ')[0].ToLowerInvariant()}.{guest.Split(' ')[^1].ToLowerInvariant()}@example.com"
            .Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u');
        var language = guest is "James Whitaker" or "Noah Bennett" or "Olivia Grant" or "Emily Carter"
            ? EPreferredLanguage.En
            : EPreferredLanguage.Es;

        var booking = Expect(await bookingCommandService.Handle(new CreateBookingCommand(propertyId, guest, email,
            null, language, checkInDate, checkInDate.AddDays(nights), 2, roomTypeId, roomId,
            _ratePlanIds[(propertyId, plan)], null), cancellationToken), $"booking of {guest}");
        return booking.Id;
    }

    private async Task ConfirmAsync(int bookingId, CancellationToken cancellationToken) =>
        Expect(await bookingCommandService.Handle(new TransitionBookingCommand(bookingId, EBookingStatus.Confirmed,
            FrontDeskAgentId), cancellationToken), $"confirmation of booking {bookingId}");

    private async Task PayShareAsync(int bookingId, decimal share, DateTimeOffset paidAt, EPaymentMethod method,
        CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.FindByIdAsync(bookingId, cancellationToken)
                      ?? throw new InvalidOperationException($"Booking {bookingId} was not saved.");
        var amount = Math.Round(booking.TotalAmount * share, 2);
        if (amount <= 0)
            return;

        Expect(await paymentCommandService.Handle(new RecordPaymentCommand(bookingId, amount, method, paidAt,
            $"DEMO-{bookingId:D4}", FrontDeskAgentId, null), cancellationToken), $"payment of booking {bookingId}");
    }

    /// <summary>
    ///     Keeps a payment moment in the past, because payments cannot be received in the future.
    /// </summary>
    private DateTimeOffset BeforeNow(DateTimeOffset moment) =>
        moment < clock.Now ? moment : clock.Now.AddMinutes(-1);

    private string NextGuestCardId() => (_guestCardNumber++).ToString("X4");

    private static TValue Expect<TValue, TError>(Result<TValue, TError> result, string subject) =>
        result.Fold(value => value,
            error => throw new InvalidOperationException($"The demonstration {subject} was rejected: {error}."));
}

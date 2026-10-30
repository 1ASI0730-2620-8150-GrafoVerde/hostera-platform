using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Model.Entities;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Rooms.Domain.Model.Aggregates;

/// <summary>
///     Represents the commercial conditions under which the room types of a property are sold.
/// </summary>
/// <remarks>
///     The name of a plan is unique within its property; that rule is checked by the application layer.
///     A plan with no room types applies to every room type of the property, including ones added later.
///     Only a refundable plan keeps a cancellation policy.
/// </remarks>
public partial class RatePlan
{
    private readonly List<RatePlanRoomType> _roomTypes = [];
    private readonly List<RatePlanService> _services = [];

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected RatePlan()
    {
        Name = null!;
    }

    /// <summary>
    ///     Creates a new RatePlan from a create command.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for CreateRatePlanCommand.
    /// </remarks>
    /// <param name="command">The CreateRatePlanCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command or one of its lists is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a required value is empty or an identifier is not valid.</exception>
    public RatePlan(CreateRatePlanCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PropertyId);

        PropertyId = command.PropertyId;
        Name = string.Empty;
        Status = ERecordStatus.Active;
        ApplyConditions(command.Name, command.Refundable, command.CancellationPolicy, command.IncludedServices,
            command.RoomTypeIds);
    }

    /// <summary>
    ///     Gets the server-generated identifier for this rate plan.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property that owns the plan.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the name of the plan.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets whether the plan can be cancelled with a refund.
    /// </summary>
    public bool Refundable { get; private set; }

    /// <summary>
    ///     Gets the cancellation conditions of a refundable plan.
    /// </summary>
    public string? CancellationPolicy { get; private set; }

    /// <summary>
    ///     Gets the status of the plan.
    /// </summary>
    public ERecordStatus Status { get; private set; }

    /// <summary>
    ///     Gets the room types linked to the plan.
    /// </summary>
    public IReadOnlyCollection<RatePlanRoomType> RoomTypes => _roomTypes.AsReadOnly();

    /// <summary>
    ///     Gets the services linked to the plan.
    /// </summary>
    public IReadOnlyCollection<RatePlanService> Services => _services.AsReadOnly();

    /// <summary>
    ///     Gets the identifiers of the room types sold under the plan.
    /// </summary>
    public IReadOnlyList<int> RoomTypeIds => _roomTypes.Select(roomType => roomType.RoomTypeId).ToList();

    /// <summary>
    ///     Gets the services included in the plan.
    /// </summary>
    public IReadOnlyList<EIncludedService> IncludedServices => _services.Select(service => service.Service).ToList();

    /// <summary>
    ///     Indicates whether a room type is sold under the plan.
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <returns>True when the plan applies to all room types or lists this one.</returns>
    public bool AppliesTo(int roomTypeId) =>
        _roomTypes.Count == 0 || _roomTypes.Any(roomType => roomType.RoomTypeId == roomTypeId);

    /// <summary>
    ///     Updates the plan from an update command.
    /// </summary>
    /// <param name="command">The UpdateRatePlanCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command or one of its lists is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a required value is empty or an identifier is not valid.</exception>
    public void Update(UpdateRatePlanCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        ApplyConditions(command.Name, command.Refundable, command.CancellationPolicy, command.IncludedServices,
            command.RoomTypeIds);
        Status = command.Status;
    }

    private void ApplyConditions(string name, bool refundable, string? cancellationPolicy,
        IReadOnlyList<EIncludedService> includedServices, IReadOnlyList<int> roomTypeIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(includedServices);
        ArgumentNullException.ThrowIfNull(roomTypeIds);

        if (includedServices.Count == 0)
            throw new ArgumentException("A rate plan must include at least one service.", nameof(includedServices));

        if (refundable && string.IsNullOrWhiteSpace(cancellationPolicy))
            throw new ArgumentException("A refundable rate plan requires a cancellation policy.",
                nameof(cancellationPolicy));

        var newRoomTypes = roomTypeIds.Distinct().Select(roomTypeId => new RatePlanRoomType(roomTypeId)).ToList();
        var newServices = includedServices.Distinct().ToList();

        Name = name.Trim();
        Refundable = refundable;
        CancellationPolicy = refundable ? cancellationPolicy!.Trim() : null;

        // Only the differences change, so EF Core never tracks a removed and a new link with the same key
        _roomTypes.RemoveAll(current => newRoomTypes.All(roomType => roomType.RoomTypeId != current.RoomTypeId));
        _roomTypes.AddRange(newRoomTypes.Where(roomType => _roomTypes.All(c => c.RoomTypeId != roomType.RoomTypeId)));
        _services.RemoveAll(current => !newServices.Contains(current.Service));
        _services.AddRange(newServices.Where(service => _services.All(c => c.Service != service))
            .Select(service => new RatePlanService(service)));
    }
}

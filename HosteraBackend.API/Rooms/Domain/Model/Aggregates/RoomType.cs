using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Rooms.Domain.Model.Aggregates;

/// <summary>
///     Represents a category of rooms of a property that share capacity, beds and base nightly rate.
/// </summary>
/// <remarks>
///     The name of a room type is unique within its property; that rule is checked by the application layer.
///     The rooms of a type take their capacity and beds from it, so a type is only removed when no room uses it.
/// </remarks>
public partial class RoomType
{
    /// <summary>
    ///     The largest number of guests a room type can host.
    /// </summary>
    public const int MaxCapacity = 12;

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected RoomType()
    {
        Name = null!;
        BedConfiguration = null!;
    }

    /// <summary>
    ///     Creates a new RoomType from a create command.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for CreateRoomTypeCommand.
    /// </remarks>
    /// <param name="command">The CreateRoomTypeCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a required value is empty or out of range.</exception>
    public RoomType(CreateRoomTypeCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PropertyId);

        PropertyId = command.PropertyId;
        Name = string.Empty;
        BedConfiguration = string.Empty;
        Status = ERecordStatus.Active;
        ApplyDetails(command.Name, command.Capacity, command.BedConfiguration, command.BaseNightlyRate);
    }

    /// <summary>
    ///     Gets the server-generated identifier for this room type.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property that owns the room type.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the name of the room type.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the number of guests a room of this type can host.
    /// </summary>
    public int Capacity { get; private set; }

    /// <summary>
    ///     Gets the description of the beds, such as "1 king + sofa bed".
    /// </summary>
    public string BedConfiguration { get; private set; }

    /// <summary>
    ///     Gets the price of a night when no daily rate replaces it.
    /// </summary>
    public decimal BaseNightlyRate { get; private set; }

    /// <summary>
    ///     Gets the status of the room type.
    /// </summary>
    public ERecordStatus Status { get; private set; }

    /// <summary>
    ///     Gets whether new rooms and bookings can use the room type.
    /// </summary>
    public bool IsActive => Status == ERecordStatus.Active;

    /// <summary>
    ///     Updates the room type from an update command.
    /// </summary>
    /// <param name="command">The UpdateRoomTypeCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a required value is empty or out of range.</exception>
    public void Update(UpdateRoomTypeCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        ApplyDetails(command.Name, command.Capacity, command.BedConfiguration, command.BaseNightlyRate);
        Status = command.Status;
    }

    private void ApplyDetails(string name, int capacity, string bedConfiguration, decimal baseNightlyRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(bedConfiguration);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(capacity, MaxCapacity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(baseNightlyRate);

        Name = name.Trim();
        Capacity = capacity;
        BedConfiguration = bedConfiguration.Trim();
        BaseNightlyRate = baseNightlyRate;
    }
}

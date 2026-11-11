using HosteraBackend.API.Rooms.Domain.Model.Commands;

namespace HosteraBackend.API.Rooms.Domain.Model.Aggregates;

/// <summary>
///     Represents a physical room of a property that guests can book.
/// </summary>
/// <remarks>
///     The number of a room is unique within its property, and its room type must belong to the same property;
///     both rules are checked by the application layer. A room takes its capacity, beds and base nightly rate from
///     its room type.
/// </remarks>
public partial class Room
{
    /// <summary>
    ///     The longest room number accepted, such as "PH-1201".
    /// </summary>
    public const int MaxNumberLength = 10;

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Room()
    {
        Number = null!;
    }

    /// <summary>
    ///     Creates a new Room from a create command.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for CreateRoomCommand.
    /// </remarks>
    /// <param name="command">The CreateRoomCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the number is empty or an identifier is not valid.</exception>
    public Room(CreateRoomCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PropertyId);

        PropertyId = command.PropertyId;
        Number = string.Empty;
        ApplyDetails(command.Number, command.RoomTypeId, command.Floor);
    }

    /// <summary>
    ///     Gets the server-generated identifier for this room.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property that owns the room.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the number of the room shown to staff and guests.
    /// </summary>
    public string Number { get; private set; }

    /// <summary>
    ///     Gets the identifier of the room type the room belongs to.
    /// </summary>
    public int RoomTypeId { get; private set; }

    /// <summary>
    ///     Gets the floor of the room, or null when it is not recorded.
    /// </summary>
    public int? Floor { get; private set; }

    /// <summary>
    ///     Updates the room from an update command.
    /// </summary>
    /// <param name="command">The UpdateRoomCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the number is empty or an identifier is not valid.</exception>
    public void Update(UpdateRoomCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ApplyDetails(command.Number, command.RoomTypeId, command.Floor);
    }

    private void ApplyDetails(string number, int roomTypeId, int? floor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomTypeId);

        var trimmedNumber = number.Trim();
        if (trimmedNumber.Length > MaxNumberLength)
            throw new ArgumentException($"A room number has at most {MaxNumberLength} characters.", nameof(number));

        Number = trimmedNumber;
        RoomTypeId = roomTypeId;
        Floor = floor;
    }
}

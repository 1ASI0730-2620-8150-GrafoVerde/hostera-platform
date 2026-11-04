using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Inventory.Domain.Model.Aggregates;

/// <summary>
///     Represents a physical area of a property where inventory items are stored and counted.
/// </summary>
/// <remarks>
///     The code of a location is unique within its property and never changes.
///     A location can only be deleted when no inventory item uses it; that rule is checked by the application layer.
/// </remarks>
public partial class StorageLocation
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected StorageLocation()
    {
        Code = null!;
        Name = null!;
        Area = null!;
        ResponsibleTeam = null!;
    }

    /// <summary>
    ///     Creates a new StorageLocation from a create command.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for CreateStorageLocationCommand.
    /// </remarks>
    /// <param name="command">The CreateStorageLocationCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the name, area or responsible team is empty.</exception>
    public StorageLocation(CreateStorageLocationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PropertyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Area);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.ResponsibleTeam);

        PropertyId = command.PropertyId;
        Code = command.Code;
        Name = command.Name.Trim();
        Type = command.Type;
        Area = command.Area.Trim();
        ResponsibleTeam = command.ResponsibleTeam.Trim();
        Description = NormalizeDescription(command.Description);
    }

    /// <summary>
    ///     Gets the server-generated identifier for this location.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property that owns the location.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the code of the location, unique within its property.
    /// </summary>
    public InventoryCode Code { get; private set; }

    /// <summary>
    ///     Gets the name of the location.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the kind of storage area.
    /// </summary>
    public EStorageLocationType Type { get; private set; }

    /// <summary>
    ///     Gets the floor or area where the location is.
    /// </summary>
    public string Area { get; private set; }

    /// <summary>
    ///     Gets the team in charge of the location.
    /// </summary>
    public string ResponsibleTeam { get; private set; }

    /// <summary>
    ///     Gets the optional description of the location.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    ///     Updates the information of this location. The code is kept.
    /// </summary>
    /// <param name="command">The UpdateStorageLocationCommand command</param>
    /// <exception cref="ArgumentException">Thrown when the name, area or responsible team is empty.</exception>
    public void Update(UpdateStorageLocationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Area);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.ResponsibleTeam);

        Name = command.Name.Trim();
        Type = command.Type;
        Area = command.Area.Trim();
        ResponsibleTeam = command.ResponsibleTeam.Trim();
        Description = NormalizeDescription(command.Description);
    }

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}

using HosteraBackend.API.Inventory.Domain.Model.Commands;
using HosteraBackend.API.Inventory.Domain.Model.Entities;
using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Inventory.Domain.Model.Aggregates;

/// <summary>
///     Represents a supply tracked by a property, with its stock by storage location and its adjustment history.
/// </summary>
/// <remarks>
///     This aggregate root keeps the stock consistent: stock never becomes negative, every change is recorded
///     as a stock adjustment, and the unit cannot change once the item has adjustments.
/// </remarks>
public partial class InventoryItem
{
    private readonly List<InventoryStock> _stocks = [];
    private readonly List<StockAdjustment> _adjustments = [];

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected InventoryItem()
    {
        Code = null!;
        Name = null!;
    }

    /// <summary>
    ///     Creates a new InventoryItem from a create command, with an empty stock in its primary location.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for CreateInventoryItemCommand.
    /// </remarks>
    /// <param name="command">The CreateInventoryItemCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the name is empty or the threshold is not valid for the unit.</exception>
    public InventoryItem(CreateInventoryItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PropertyId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PrimaryLocationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Name);

        PropertyId = command.PropertyId;
        Code = command.Code;
        Name = command.Name.Trim();
        Category = command.Category;
        Unit = command.Unit;
        PrimaryLocationId = command.PrimaryLocationId;
        LowStockThreshold = new StockQuantity(command.LowStockThreshold, command.Unit).Value;
        _stocks.Add(new InventoryStock(command.PrimaryLocationId));
    }

    /// <summary>
    ///     Gets the server-generated identifier for this item.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property that owns the item.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the code of the item, unique within its property.
    /// </summary>
    public InventoryCode Code { get; private set; }

    /// <summary>
    ///     Gets the name of the item.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the category of the item.
    /// </summary>
    public EInventoryCategory Category { get; private set; }

    /// <summary>
    ///     Gets the unit in which the item is counted.
    /// </summary>
    public EInventoryUnit Unit { get; private set; }

    /// <summary>
    ///     Gets the identifier of the storage location where the item is usually kept.
    /// </summary>
    public int PrimaryLocationId { get; private set; }

    /// <summary>
    ///     Gets the quantity at which the item needs replenishment.
    /// </summary>
    public decimal LowStockThreshold { get; private set; }

    /// <summary>
    ///     Gets the stock of the item in each storage location.
    /// </summary>
    public IReadOnlyCollection<InventoryStock> Stocks => _stocks.AsReadOnly();

    /// <summary>
    ///     Gets the recorded stock adjustments of the item.
    /// </summary>
    public IReadOnlyCollection<StockAdjustment> Adjustments => _adjustments.AsReadOnly();

    /// <summary>
    ///     Gets the quantity on hand across all storage locations.
    /// </summary>
    public decimal TotalQuantity => _stocks.Sum(stock => stock.Quantity);

    /// <summary>
    ///     Gets the condition of the item compared with its low stock threshold.
    /// </summary>
    public EStockCondition StockCondition => TotalQuantity == 0
        ? EStockCondition.OutOfStock
        : TotalQuantity <= LowStockThreshold
            ? EStockCondition.LowStock
            : EStockCondition.InStock;

    /// <summary>
    ///     Gets the quantity on hand in a storage location.
    /// </summary>
    /// <param name="storageLocationId">The identifier of the storage location.</param>
    /// <returns>The quantity on hand, or zero when the item has no stock record there.</returns>
    public decimal QuantityAt(int storageLocationId) =>
        _stocks.FirstOrDefault(stock => stock.StorageLocationId == storageLocationId)?.Quantity ?? 0;

    /// <summary>
    ///     Updates the information of this item. The code is kept.
    /// </summary>
    /// <param name="command">The UpdateInventoryItemCommand command</param>
    /// <exception cref="ArgumentException">Thrown when the name is empty or the threshold is not valid for the unit.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the unit changes after the item has adjustments.</exception>
    public void Update(UpdateInventoryItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PrimaryLocationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Name);

        if (command.Unit != Unit && _adjustments.Count > 0)
            throw new InvalidOperationException("The unit of an item cannot change after its first stock adjustment.");

        var threshold = new StockQuantity(command.LowStockThreshold, command.Unit).Value;

        Name = command.Name.Trim();
        Category = command.Category;
        Unit = command.Unit;
        PrimaryLocationId = command.PrimaryLocationId;
        LowStockThreshold = threshold;
        FindOrAddStock(command.PrimaryLocationId);
    }

    /// <summary>
    ///     Records stock entering, leaving or moving between storage locations.
    /// </summary>
    /// <param name="command">The AdjustStockCommand command</param>
    /// <param name="recordedAt">The moment the movement is recorded.</param>
    /// <returns>The adjustments created: one for stock in or out, two for a transfer.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when the quantity, the reason, the note, the operator or the destination is not valid.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown when the stock on hand would become negative.</exception>
    public IReadOnlyList<StockAdjustment> AdjustStock(AdjustStockCommand command, DateTimeOffset recordedAt)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.StorageLocationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Operator);

        if (command.Quantity <= 0)
            throw new ArgumentException("The adjusted quantity must be greater than zero.", nameof(command));

        var quantity = new StockQuantity(command.Quantity, Unit).Value;

        if (!command.Reason.IsValidFor(command.Operation))
            throw new ArgumentException("The reason is not valid for the operation.", nameof(command));

        if (command.Reason == EStockAdjustmentReason.Other && string.IsNullOrWhiteSpace(command.Note))
            throw new ArgumentException("A note is required when the reason is Other.", nameof(command));

        List<StockAdjustment> created = command.Operation switch
        {
            EStockOperation.In => [RecordIn(command, quantity, recordedAt)],
            EStockOperation.Out => [RecordOut(command, quantity, recordedAt)],
            EStockOperation.Transfer => RecordTransfer(command, quantity, recordedAt),
            _ => throw new ArgumentException("The operation is not supported.", nameof(command))
        };

        _adjustments.AddRange(created);
        return created;
    }

    private StockAdjustment RecordIn(AdjustStockCommand command, decimal quantity, DateTimeOffset recordedAt)
    {
        FindOrAddStock(command.StorageLocationId).Increase(quantity);
        return new StockAdjustment(EStockOperation.In, quantity, command.StorageLocationId, command.Reason,
            command.Note, command.Operator, recordedAt, null);
    }

    private StockAdjustment RecordOut(AdjustStockCommand command, decimal quantity, DateTimeOffset recordedAt)
    {
        RemoveStock(command.StorageLocationId, quantity);
        return new StockAdjustment(EStockOperation.Out, -quantity, command.StorageLocationId, command.Reason,
            command.Note, command.Operator, recordedAt, null);
    }

    private List<StockAdjustment> RecordTransfer(AdjustStockCommand command, decimal quantity, DateTimeOffset recordedAt)
    {
        if (command.DestinationStorageLocationId is not { } destinationId || destinationId <= 0)
            throw new ArgumentException("A transfer requires a destination location.", nameof(command));

        if (destinationId == command.StorageLocationId)
            throw new ArgumentException("The destination must be different from the source location.", nameof(command));

        RemoveStock(command.StorageLocationId, quantity);
        FindOrAddStock(destinationId).Increase(quantity);

        var transferId = Guid.NewGuid();
        return
        [
            new StockAdjustment(EStockOperation.Transfer, -quantity, command.StorageLocationId, command.Reason,
                command.Note, command.Operator, recordedAt, transferId),
            new StockAdjustment(EStockOperation.Transfer, quantity, destinationId, command.Reason,
                command.Note, command.Operator, recordedAt, transferId)
        ];
    }

    private void RemoveStock(int storageLocationId, decimal quantity)
    {
        var stock = _stocks.FirstOrDefault(s => s.StorageLocationId == storageLocationId)
                    ?? throw new InvalidOperationException("The item has no stock in the location.");
        stock.Decrease(quantity);
    }

    private InventoryStock FindOrAddStock(int storageLocationId)
    {
        var stock = _stocks.FirstOrDefault(s => s.StorageLocationId == storageLocationId);
        if (stock is not null) return stock;

        stock = new InventoryStock(storageLocationId);
        _stocks.Add(stock);
        return stock;
    }
}

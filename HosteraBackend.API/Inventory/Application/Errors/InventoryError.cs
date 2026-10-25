namespace HosteraBackend.API.Inventory.Application.Errors;

/// <summary>
/// Represents errors that can occur in the inventory operations.
/// </summary>
public enum InventoryError
{
	/// <summary>
	/// One or more provided values are invalid, such as an empty name or a negative quantity.
	/// </summary>
	InvalidInput,

	/// <summary>
	/// The requested storage location does not exist.
	/// </summary>
	StorageLocationNotFound,

	/// <summary>
	/// The requested inventory item does not exist.
	/// </summary>
	InventoryItemNotFound,

	/// <summary>
	/// The storage location belongs to a different property than the inventory item.
	/// </summary>
	StorageLocationFromAnotherProperty,

	/// <summary>
	/// Another storage location of the property already uses the code.
	/// </summary>
	DuplicateStorageLocationCode,

	/// <summary>
	/// Another inventory item of the property already uses the code.
	/// </summary>
	DuplicateInventoryItemCode,

	/// <summary>
	/// The storage location cannot be deleted because inventory items are assigned to it.
	/// </summary>
	StorageLocationInUse,

	/// <summary>
	/// The unit of the item cannot change because it already has stock adjustments.
	/// </summary>
	UnitChangeNotAllowed,

	/// <summary>
	/// The reason is not valid for the operation, or a note is missing for the Other reason.
	/// </summary>
	InvalidAdjustmentReason,

	/// <summary>
	/// The requested quantity exceeds the stock on hand in the location.
	/// </summary>
	InsufficientStock,

	/// <summary>
	/// An unexpected error occurred during the operation.
	/// </summary>
	UnexpectedError
}

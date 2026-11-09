namespace HosteraBackend.API.Shared.Resources;

/// <summary>
///     Marker type of the localized resources with the messages shared by every bounded context.
/// </summary>
/// <remarks>
///     The English texts live in <c>CommonMessages.resx</c> and the Spanish texts in <c>CommonMessages.es.resx</c>. Each error message
///     is keyed by its error type and value, such as <c>RoomError.DuplicateRoomNumber</c>.
/// </remarks>
public class CommonMessages;

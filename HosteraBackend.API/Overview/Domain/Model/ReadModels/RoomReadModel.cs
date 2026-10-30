namespace HosteraBackend.API.Overview.Domain.Model.ReadModels;

/// <summary>
///     Room of a property, as the Overview bounded context reads it from the Rooms bounded context
/// </summary>
/// <param name="Id">The identifier of the room</param>
/// <param name="Number">The number of the room</param>
public record RoomReadModel(int Id, string Number);

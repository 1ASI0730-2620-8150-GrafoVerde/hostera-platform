namespace HosteraBackend.API.Overview.Domain.Model.ReadModels;

/// <summary>
///     Property, as the Overview bounded context reads it from the Rooms bounded context
/// </summary>
/// <param name="Id">The identifier of the property</param>
/// <param name="Name">The name of the property</param>
public record PropertyReadModel(int Id, string Name);

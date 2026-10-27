using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Domain.Repositories;

/// <summary>
///     The property repository contract.
/// </summary>
public interface IPropertyRepository : IBaseRepository<Property>;

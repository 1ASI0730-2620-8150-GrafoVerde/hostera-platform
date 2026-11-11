using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;

namespace HosteraBackend.API.AccessControl.Application.QueryServices;

/// <summary>
///     A page of the access events that satisfy a search.
/// </summary>
/// <param name="AccessEvents">The access events of the page, starting with the most recent</param>
/// <param name="Page">The number of the page, starting at 1</param>
/// <param name="PageSize">The number of access events per page</param>
/// <param name="TotalCount">The number of access events that satisfy the search across every page</param>
public record AccessEventPage(IEnumerable<AccessEvent> AccessEvents, int Page, int PageSize, int TotalCount);

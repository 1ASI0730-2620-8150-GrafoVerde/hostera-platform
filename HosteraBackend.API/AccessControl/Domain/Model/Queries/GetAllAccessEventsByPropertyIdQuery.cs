using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Queries;

/// <summary>
///     Query to get a page of the access events of a property that satisfy the given criteria
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
/// <param name="From">The first moment of the search, or null for no lower limit</param>
/// <param name="To">The last moment of the search, or null for no upper limit</param>
/// <param name="Result">The result to search, or null for any result</param>
/// <param name="HolderName">Part of the name of the holder to search, or null for any person</param>
/// <param name="CredentialId">The credential ID to search, or null for any credential</param>
/// <param name="CardId">The card identifier to search, or null for any card</param>
/// <param name="AccessPoint">The access point to search, or null for any access point</param>
/// <param name="RoomId">The room ID to search, or null for any room</param>
/// <param name="Page">The number of the page, starting at 1</param>
/// <param name="PageSize">The number of access events per page</param>
public record GetAllAccessEventsByPropertyIdQuery(
    int PropertyId,
    DateTimeOffset? From,
    DateTimeOffset? To,
    EAccessResult? Result,
    string? HolderName,
    int? CredentialId,
    CardId? CardId,
    EAccessPoint? AccessPoint,
    int? RoomId,
    int Page,
    int PageSize);

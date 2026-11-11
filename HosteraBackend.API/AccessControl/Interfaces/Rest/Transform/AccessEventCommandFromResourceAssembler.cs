using HosteraBackend.API.AccessControl.Domain.Model.Commands;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming the access event resources into their commands.
/// </summary>
public static class AccessEventCommandFromResourceAssembler
{
    /// <summary>
    ///     Indicates whether a reader reported one of the access points registered in the properties.
    /// </summary>
    /// <param name="accessPoint">The kebab-case name of the access point, such as "room-door".</param>
    /// <returns>True when the name matches a registered access point.</returns>
    public static bool IsRegisteredAccessPoint(string? accessPoint)
    {
        try
        {
            AccessControlEnumNames.Parse<EAccessPoint>(accessPoint, "access point");
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Converts a <see cref="RecordAccessEventResource" /> to a <see cref="RecordAccessEventCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data read by the reader. Must not be null.</param>
    /// <returns>A new <see cref="RecordAccessEventCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the card identifier or the access point is not valid.</exception>
    public static RecordAccessEventCommand ToCommandFromResource(RecordAccessEventResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new RecordAccessEventCommand(
            resource.PropertyId,
            new CardId(resource.CardId),
            AccessControlEnumNames.Parse<EAccessPoint>(resource.AccessPoint, "access point"),
            resource.RoomId,
            resource.OccurredAt ?? DateTimeOffset.UtcNow);
    }
}

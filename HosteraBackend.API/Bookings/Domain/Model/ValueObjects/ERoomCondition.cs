namespace HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

/// <summary>
///     The condition of the room recorded at check-out.
/// </summary>
public enum ERoomCondition
{
    /// <summary>
    ///     No issue was reported.
    /// </summary>
    NoIssues,

    /// <summary>
    ///     Something in the room needs attention, described in the departure note.
    /// </summary>
    NeedsAttention
}

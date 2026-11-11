namespace HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

/// <summary>
///     The original identity document verified at check-in.
/// </summary>
public enum EDocumentType
{
    /// <summary>
    ///     The Peruvian national identity document.
    /// </summary>
    Dni,

    /// <summary>
    ///     A passport.
    /// </summary>
    Passport,

    /// <summary>
    ///     A foreign resident card.
    /// </summary>
    ForeignResidentCard
}

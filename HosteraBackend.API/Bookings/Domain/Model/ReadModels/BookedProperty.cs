namespace HosteraBackend.API.Bookings.Domain.Model.ReadModels;

/// <summary>
///     The property of a booking, as the Bookings context reads it from Rooms.
/// </summary>
/// <param name="Id">The identifier of the property.</param>
/// <param name="Name">The name of the property, shown to the guest.</param>
/// <param name="Currency">The three-letter code of the currency in which the property prices its stays.</param>
public record BookedProperty(int Id, string Name, string Currency);

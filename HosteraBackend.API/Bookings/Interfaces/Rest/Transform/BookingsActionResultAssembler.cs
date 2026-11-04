using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Transform;

/// <summary>
///     Builds the HTTP responses of the Bookings endpoints, turning each <see cref="BookingError" /> into a problem
///     details response with its status code.
/// </summary>
public static class BookingsActionResultAssembler
{
    private static int ToStatusCodeFromBookingError(BookingError error) => error switch
    {
        BookingError.BookingNotFound => StatusCodes.Status404NotFound,
        BookingError.RoomNotFound => StatusCodes.Status404NotFound,
        BookingError.RoomTypeNotFound => StatusCodes.Status404NotFound,
        BookingError.RatePlanNotFound => StatusCodes.Status404NotFound,
        BookingError.InactiveRoomType => StatusCodes.Status409Conflict,
        BookingError.InactiveRatePlan => StatusCodes.Status409Conflict,
        BookingError.RoomUnavailable => StatusCodes.Status409Conflict,
        BookingError.NotEditable => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ToMessageFromBookingError(BookingError error) => error switch
    {
        BookingError.InvalidInput => "One or more values are not valid.",
        BookingError.InvalidStayDates =>
            "The check-out day must be after the check-in day, and a stay covers at most 365 nights.",
        BookingError.BookingNotFound => "The specified booking was not found.",
        BookingError.RoomNotFound => "The room does not exist in the property of the booking.",
        BookingError.RoomTypeNotFound => "The room type does not exist in the property of the booking.",
        BookingError.RatePlanNotFound => "The rate plan does not exist in the property of the booking.",
        BookingError.RoomNotInRoomType => "The room does not belong to the booked room type.",
        BookingError.InactiveRoomType => "The room type is inactive, so new bookings cannot use it.",
        BookingError.InactiveRatePlan => "The rate plan is inactive, so new bookings cannot use it.",
        BookingError.RatePlanNotForRoomType => "The rate plan does not sell the booked room type.",
        BookingError.OverCapacity => "The number of guests exceeds the capacity of the room type.",
        BookingError.RoomUnavailable =>
            "Another booking holds the room, or a Blocked or Out of service status covers it, on a night of the stay.",
        BookingError.NotEditable => "Only pending or confirmed bookings can be edited.",
        _ => "An unexpected error occurred."
    };

    /// <summary>
    ///     Returns the success action, or a problem details response when the result is a failure.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <param name="result">The result of the command.</param>
    /// <param name="successAction">The action that builds the response for the created or updated value.</param>
    /// <typeparam name="TValue">The type of the value returned by the command.</typeparam>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromResult<TValue>(
        ControllerBase controller,
        Result<TValue, BookingError> result,
        Func<TValue, IActionResult> successAction)
    {
        return result.Fold(successAction, error => ToProblemFromBookingError(controller, error));
    }

    /// <summary>
    ///     Returns the success action, or a not found problem details response when the value is null.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <param name="value">The value found by the query, or null.</param>
    /// <param name="notFoundError">The error to report when the value is null.</param>
    /// <param name="successAction">The action that builds the response for the found value.</param>
    /// <typeparam name="TValue">The type of the value returned by the query.</typeparam>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromQueryResult<TValue>(
        ControllerBase controller,
        TValue? value,
        BookingError notFoundError,
        Func<TValue, IActionResult> successAction) where TValue : class
    {
        return value is null ? ToProblemFromBookingError(controller, notFoundError) : successAction(value);
    }

    /// <summary>
    ///     Returns a bad request problem details response for a resource that cannot become a command.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <param name="exception">The exception raised while reading the resource.</param>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromInvalidResource(ControllerBase controller, ArgumentException exception)
    {
        return controller.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: nameof(BookingError.InvalidInput),
            detail: exception.Message);
    }

    private static IActionResult ToProblemFromBookingError(ControllerBase controller, BookingError error)
    {
        return controller.Problem(
            statusCode: ToStatusCodeFromBookingError(error),
            title: error.ToString(),
            detail: ToMessageFromBookingError(error));
    }
}

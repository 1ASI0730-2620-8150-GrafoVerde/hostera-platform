using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Bookings.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

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
        BookingError.InvalidStatusChange => StatusCodes.Status409Conflict,
        BookingError.PaymentsNotAccepted => StatusCodes.Status409Conflict,
        BookingError.InvalidPaymentAmount => StatusCodes.Status422UnprocessableEntity,
        BookingError.PaymentExceedsBalance => StatusCodes.Status422UnprocessableEntity,
        BookingError.IdempotencyKeyReused => StatusCodes.Status409Conflict,
        BookingError.KeyCardsNotIssued => StatusCodes.Status409Conflict,
        BookingError.BalanceDue => StatusCodes.Status409Conflict,
        BookingError.ConcurrencyConflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ToMessageFromBookingError(ControllerBase controller, BookingError error)
    {
        var localizer = controller.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<BookingsMessages>>();
        return localizer[$"{nameof(BookingError)}.{error}"];
    }

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

    /// <summary>
    ///     Returns a conflict problem details response for a status change that the booking does not allow, with its
    ///     current status and the statuses it can move to.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <param name="booking">The booking whose status could not change.</param>
    /// <param name="allowedTransitions">The statuses the booking can move to today.</param>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromInvalidTransition(ControllerBase controller, Booking booking,
        IEnumerable<EBookingStatus> allowedTransitions)
    {
        var problem = controller.ProblemDetailsFactory.CreateProblemDetails(
            controller.HttpContext,
            StatusCodes.Status409Conflict,
            nameof(BookingError.InvalidStatusChange),
            detail: ToMessageFromBookingError(controller, BookingError.InvalidStatusChange));
        problem.Extensions["currentStatus"] = BookingsEnumNames.ToName(booking.Status);
        problem.Extensions["allowedTransitions"] = allowedTransitions.Select(BookingsEnumNames.ToName).ToList();
        return new ObjectResult(problem) { StatusCode = StatusCodes.Status409Conflict };
    }

    private static IActionResult ToProblemFromBookingError(ControllerBase controller, BookingError error)
    {
        return controller.Problem(
            statusCode: ToStatusCodeFromBookingError(error),
            title: error.ToString(),
            detail: ToMessageFromBookingError(controller, error));
    }
}

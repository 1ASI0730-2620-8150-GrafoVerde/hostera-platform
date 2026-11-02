using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Builds the HTTP responses of the Rooms endpoints, turning each <see cref="RoomError" /> and
///     <see cref="RateError" /> into a problem details response with its status code.
/// </summary>
public static class RoomsActionResultAssembler
{
    private static int ToStatusCodeFromRoomError(RoomError error) => error switch
    {
        RoomError.PropertyNotFound => StatusCodes.Status404NotFound,
        RoomError.RoomTypeNotFound => StatusCodes.Status404NotFound,
        RoomError.DuplicateRoomTypeName => StatusCodes.Status409Conflict,
        RoomError.RoomNotFound => StatusCodes.Status404NotFound,
        RoomError.DuplicateRoomNumber => StatusCodes.Status409Conflict,
        RoomError.InactiveRoomType => StatusCodes.Status409Conflict,
        RoomError.RoomTypeInUse => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ToMessageFromRoomError(RoomError error) => error switch
    {
        RoomError.InvalidInput => "One or more values are not valid.",
        RoomError.PropertyNotFound => "The specified property was not found.",
        RoomError.RoomTypeNotFound => "The specified room type was not found.",
        RoomError.DuplicateRoomTypeName => "Another room type of the property uses the same name.",
        RoomError.RoomNotFound => "The specified room was not found.",
        RoomError.DuplicateRoomNumber => "Another room of the property uses the same number.",
        RoomError.RoomTypeFromAnotherProperty => "The room type belongs to a different property than the room.",
        RoomError.InactiveRoomType => "The room type is inactive, so rooms cannot be assigned to it.",
        RoomError.RoomTypeInUse =>
            "The room type cannot be deleted while a room belongs to it or a rate plan lists it.",
        RoomError.InvalidDateRange => "The date range ends before it starts or is longer than 366 days.",
        _ => "An unexpected error occurred."
    };

    private static int ToStatusCodeFromRateError(RateError error) => error switch
    {
        RateError.RatePlanNotFound => StatusCodes.Status404NotFound,
        RateError.PropertyNotFound => StatusCodes.Status404NotFound,
        RateError.DuplicateRatePlanName => StatusCodes.Status409Conflict,
        RateError.RoomTypeNotInRatePlan => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ToMessageFromRateError(RateError error) => error switch
    {
        RateError.InvalidInput =>
            "One or more values are not valid, or a refundable plan has no cancellation policy.",
        RateError.InvalidDateRange => "The date range ends before it starts or is longer than 366 nights.",
        RateError.RatePlanNotFound => "The specified rate plan was not found.",
        RateError.DuplicateRatePlanName => "Another rate plan of the property uses the same name.",
        RateError.RoomTypeNotInRatePlan => "The room type is not sold under the rate plan.",
        RateError.PropertyNotFound => "The specified property was not found.",
        RateError.UnknownRoomType => "One or more room types do not exist in the property of the rate plan.",
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
        Result<TValue, RoomError> result,
        Func<TValue, IActionResult> successAction)
    {
        return result.Fold(successAction, error => ToProblemFromRoomError(controller, error));
    }

    /// <summary>
    ///     Returns the success action, or a problem details response when the rate result is a failure.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <param name="result">The result of the rate command.</param>
    /// <param name="successAction">The action that builds the response for the created or updated value.</param>
    /// <typeparam name="TValue">The type of the value returned by the command.</typeparam>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromResult<TValue>(
        ControllerBase controller,
        Result<TValue, RateError> result,
        Func<TValue, IActionResult> successAction)
    {
        return result.Fold(successAction, error => ToProblemFromRateError(controller, error));
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
        RoomError notFoundError,
        Func<TValue, IActionResult> successAction) where TValue : class
    {
        return value is null ? ToProblemFromRoomError(controller, notFoundError) : successAction(value);
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
            title: nameof(RoomError.InvalidInput),
            detail: exception.Message);
    }

    private static IActionResult ToProblemFromRoomError(ControllerBase controller, RoomError error)
    {
        return controller.Problem(
            statusCode: ToStatusCodeFromRoomError(error),
            title: error.ToString(),
            detail: ToMessageFromRoomError(error));
    }

    /// <summary>
    ///     Returns the problem details response of a rate error.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <param name="error">The rate error to report.</param>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromRateError(ControllerBase controller, RateError error)
    {
        return ToProblemFromRateError(controller, error);
    }

    private static IActionResult ToProblemFromRateError(ControllerBase controller, RateError error)
    {
        return controller.Problem(
            statusCode: ToStatusCodeFromRateError(error),
            title: error.ToString(),
            detail: ToMessageFromRateError(error));
    }
}

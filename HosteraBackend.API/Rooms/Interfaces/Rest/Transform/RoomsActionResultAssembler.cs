using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Builds the HTTP responses of the Rooms endpoints, turning each <see cref="RoomError" /> into a problem
///     details response with its status code.
/// </summary>
public static class RoomsActionResultAssembler
{
    private static int ToStatusCodeFromRoomError(RoomError error) => error switch
    {
        RoomError.PropertyNotFound => StatusCodes.Status404NotFound,
        RoomError.RoomTypeNotFound => StatusCodes.Status404NotFound,
        RoomError.DuplicateRoomTypeName => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ToMessageFromRoomError(RoomError error) => error switch
    {
        RoomError.InvalidInput => "One or more values are not valid.",
        RoomError.PropertyNotFound => "The specified property was not found.",
        RoomError.RoomTypeNotFound => "The specified room type was not found.",
        RoomError.DuplicateRoomTypeName => "Another room type of the property uses the same name.",
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
}

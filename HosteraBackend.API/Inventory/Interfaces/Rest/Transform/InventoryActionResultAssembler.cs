using HosteraBackend.API.Inventory.Application.Errors;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Inventory.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Transform;

/// <summary>
///     Builds the HTTP responses of the Inventory endpoints, turning each <see cref="InventoryError" /> into a
///     problem details response with its status code.
/// </summary>
public static class InventoryActionResultAssembler
{
    private static int ToStatusCodeFromInventoryError(InventoryError error) => error switch
    {
        InventoryError.StorageLocationNotFound => StatusCodes.Status404NotFound,
        InventoryError.InventoryItemNotFound => StatusCodes.Status404NotFound,
        InventoryError.DuplicateStorageLocationCode => StatusCodes.Status409Conflict,
        InventoryError.DuplicateInventoryItemCode => StatusCodes.Status409Conflict,
        InventoryError.StorageLocationInUse => StatusCodes.Status409Conflict,
        InventoryError.StorageLocationNotAssigned => StatusCodes.Status404NotFound,
        InventoryError.StorageLocationAssignmentInUse => StatusCodes.Status409Conflict,
        InventoryError.InsufficientStock => StatusCodes.Status409Conflict,
        InventoryError.UnitChangeNotAllowed => StatusCodes.Status409Conflict,
        InventoryError.UnexpectedError => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ToMessageFromInventoryError(ControllerBase controller, InventoryError error)
    {
        var localizer = controller.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<InventoryMessages>>();
        return localizer[$"{nameof(InventoryError)}.{error}"];
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
        Result<TValue, InventoryError> result,
        Func<TValue, IActionResult> successAction)
    {
        return result.Fold(successAction, error => ToProblemFromInventoryError(controller, error));
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
        InventoryError notFoundError,
        Func<TValue, IActionResult> successAction) where TValue : class
    {
        return value is null ? ToProblemFromInventoryError(controller, notFoundError) : successAction(value);
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
            title: nameof(InventoryError.InvalidInput),
            detail: exception.Message);
    }

    private static IActionResult ToProblemFromInventoryError(ControllerBase controller, InventoryError error)
    {
        return controller.Problem(
            statusCode: ToStatusCodeFromInventoryError(error),
            title: error.ToString(),
            detail: ToMessageFromInventoryError(controller, error));
    }
}

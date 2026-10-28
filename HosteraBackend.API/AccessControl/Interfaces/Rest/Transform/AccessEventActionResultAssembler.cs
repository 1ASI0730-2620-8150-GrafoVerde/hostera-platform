using HosteraBackend.API.AccessControl.Application.Errors;
using HosteraBackend.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;

/// <summary>
///     Builds the HTTP responses of the access event endpoints, turning each <see cref="AccessEventError" /> into a
///     problem details response with its status code.
/// </summary>
public static class AccessEventActionResultAssembler
{
    private static int ToStatusCodeFromAccessEventError(AccessEventError error) => error switch
    {
        AccessEventError.AccessPointNotRegistered => StatusCodes.Status403Forbidden,
        AccessEventError.CredentialNotFound => StatusCodes.Status404NotFound,
        AccessEventError.AccessEventNotFound => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ToMessageFromAccessEventError(AccessEventError error) => error switch
    {
        AccessEventError.AccessPointNotRegistered => "The access point of the reader is not registered.",
        AccessEventError.CredentialNotFound => "No credential of the property was written to the card.",
        AccessEventError.AccessEventNotFound => "The specified access event was not found.",
        _ => "One or more values are not valid."
    };

    /// <summary>
    ///     Returns the success action, or a problem details response when the result is a failure.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <param name="result">The result of the command.</param>
    /// <param name="successAction">The action that builds the response for the returned value.</param>
    /// <typeparam name="TValue">The type of the value returned by the command.</typeparam>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromResult<TValue>(
        ControllerBase controller,
        Result<TValue, AccessEventError> result,
        Func<TValue, IActionResult> successAction)
    {
        return result.Fold(successAction, error => ToProblemFromAccessEventError(controller, error));
    }

    /// <summary>
    ///     Returns a not found problem details response for an access event that does not exist.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromMissingAccessEvent(ControllerBase controller)
    {
        return ToProblemFromAccessEventError(controller, AccessEventError.AccessEventNotFound);
    }

    /// <summary>
    ///     Returns a forbidden problem details response for a reader whose access point is not registered.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromUnregisteredAccessPoint(ControllerBase controller)
    {
        return ToProblemFromAccessEventError(controller, AccessEventError.AccessPointNotRegistered);
    }

    /// <summary>
    ///     Returns a bad request problem details response for a resource or a search that cannot be read.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <param name="exception">The exception raised while reading the resource or the search.</param>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromInvalidResource(ControllerBase controller, ArgumentException exception)
    {
        return controller.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: nameof(AccessEventError.InvalidInput),
            detail: exception.Message);
    }

    private static IActionResult ToProblemFromAccessEventError(ControllerBase controller, AccessEventError error)
    {
        return controller.Problem(
            statusCode: ToStatusCodeFromAccessEventError(error),
            title: error.ToString(),
            detail: ToMessageFromAccessEventError(error));
    }
}

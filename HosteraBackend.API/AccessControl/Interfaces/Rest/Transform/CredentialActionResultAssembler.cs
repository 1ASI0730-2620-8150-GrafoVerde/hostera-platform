using HosteraBackend.API.AccessControl.Application.Errors;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.AccessControl.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;

/// <summary>
///     Builds the HTTP responses of the credential endpoints, turning each <see cref="CredentialError" /> into a
///     problem details response with its status code.
/// </summary>
public static class CredentialActionResultAssembler
{
    private static int ToStatusCodeFromCredentialError(CredentialError error) => error switch
    {
        CredentialError.CredentialNotFound => StatusCodes.Status404NotFound,
        CredentialError.StaffMemberNotFound => StatusCodes.Status404NotFound,
        CredentialError.StaffCredentialExists => StatusCodes.Status409Conflict,
        CredentialError.DuplicateCardId => StatusCodes.Status409Conflict,
        CredentialError.CredentialNotUsable => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ToMessageFromCredentialError(ControllerBase controller, CredentialError error)
    {
        var localizer = controller.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<AccessControlMessages>>();
        return localizer[$"{nameof(CredentialError)}.{error}"];
    }

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
        Result<TValue, CredentialError> result,
        Func<TValue, IActionResult> successAction)
    {
        return result.Fold(successAction, error => ToProblemFromCredentialError(controller, error));
    }

    /// <summary>
    ///     Returns a not found problem details response for a credential that does not exist.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromMissingCredential(ControllerBase controller)
    {
        return ToProblemFromCredentialError(controller, CredentialError.CredentialNotFound);
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
            title: nameof(CredentialError.InvalidInput),
            detail: exception.Message);
    }

    private static IActionResult ToProblemFromCredentialError(ControllerBase controller, CredentialError error)
    {
        return controller.Problem(
            statusCode: ToStatusCodeFromCredentialError(error),
            title: error.ToString(),
            detail: ToMessageFromCredentialError(controller, error));
    }
}

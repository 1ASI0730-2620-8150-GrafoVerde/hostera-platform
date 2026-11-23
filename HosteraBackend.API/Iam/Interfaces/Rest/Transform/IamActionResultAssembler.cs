using HosteraBackend.API.Iam.Application.Errors;
using HosteraBackend.API.Iam.Resources;
using HosteraBackend.API.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HosteraBackend.API.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Builds the HTTP responses of the IAM endpoints, turning each <see cref="IamError" /> into a problem details
///     response with its status code.
/// </summary>
public static class IamActionResultAssembler
{
    private static int ToStatusCodeFromIamError(IamError error) => error switch
    {
        IamError.UserNotFound => StatusCodes.Status404NotFound,
        IamError.UsernameAlreadyTaken => StatusCodes.Status409Conflict,
        IamError.InvalidCredentials => StatusCodes.Status401Unauthorized,
        IamError.AuthenticationRequired => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ToMessageFromIamError(ControllerBase controller, IamError error)
    {
        var localizer = controller.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<IamMessages>>();
        return localizer[$"{nameof(IamError)}.{error}"];
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
        Result<TValue, IamError> result,
        Func<TValue, IActionResult> successAction)
    {
        return result.Fold(successAction, error => ToProblemFromIamError(controller, error));
    }

    /// <summary>
    ///     Returns a not found problem details response for a user that does not exist.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromMissingUser(ControllerBase controller)
    {
        return ToProblemFromIamError(controller, IamError.UserNotFound);
    }

    private static IActionResult ToProblemFromIamError(ControllerBase controller, IamError error)
    {
        return controller.Problem(
            statusCode: ToStatusCodeFromIamError(error),
            title: error.ToString(),
            detail: ToMessageFromIamError(controller, error));
    }
}

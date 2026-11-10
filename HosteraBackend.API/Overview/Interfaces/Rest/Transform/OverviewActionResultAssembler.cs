using HosteraBackend.API.Overview.Application.Errors;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Overview.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HosteraBackend.API.Overview.Interfaces.Rest.Transform;

/// <summary>
///     Builds the HTTP responses of the overview endpoints, turning each <see cref="OverviewError" /> into a
///     problem details response with its status code.
/// </summary>
public static class OverviewActionResultAssembler
{
    private static int ToStatusCodeFromOverviewError(OverviewError error) => error switch
    {
        OverviewError.PropertyNotFound => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status400BadRequest
    };

    private static string ToMessageFromOverviewError(ControllerBase controller, OverviewError error)
    {
        var localizer = controller.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<OverviewMessages>>();
        return localizer[$"{nameof(OverviewError)}.{error}"];
    }

    /// <summary>
    ///     Returns the success action, or a problem details response when the result is a failure.
    /// </summary>
    /// <param name="controller">The controller that handles the request.</param>
    /// <param name="result">The result of the query.</param>
    /// <param name="successAction">The action that builds the response for the returned value.</param>
    /// <typeparam name="TValue">The type of the value returned by the query.</typeparam>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromResult<TValue>(
        ControllerBase controller,
        Result<TValue, OverviewError> result,
        Func<TValue, IActionResult> successAction)
    {
        return result.Fold(successAction, error => controller.Problem(
            statusCode: ToStatusCodeFromOverviewError(error),
            title: error.ToString(),
            detail: ToMessageFromOverviewError(controller, error)));
    }
}

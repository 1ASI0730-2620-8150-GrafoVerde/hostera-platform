using HosteraBackend.API.Shared.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;

namespace HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.Configuration;

/// <summary>
///     Builds the response of a request whose body or parameters fail model validation.
/// </summary>
/// <remarks>
///     The response keeps the field errors of the validation problem details, names the problem
///     <c>InvalidInput</c> like the errors of every bounded context, and describes it in the language of the request.
/// </remarks>
public static class InvalidModelStateProblemFactory
{
    private const string InvalidInput = "InvalidInput";

    /// <summary>
    ///     Returns a bad request validation problem details response for the invalid model state of a request.
    /// </summary>
    /// <param name="context">The context of the action whose model state is invalid.</param>
    /// <returns>The HTTP response.</returns>
    public static IActionResult ToActionResultFromInvalidModelState(ActionContext context)
    {
        var services = context.HttpContext.RequestServices;
        var localizer = services.GetRequiredService<IStringLocalizer<CommonMessages>>();
        var problem = services.GetRequiredService<ProblemDetailsFactory>().CreateValidationProblemDetails(
            context.HttpContext,
            context.ModelState,
            StatusCodes.Status400BadRequest,
            InvalidInput,
            detail: localizer[InvalidInput]);
        return new BadRequestObjectResult(problem) { ContentTypes = { "application/problem+json" } };
    }
}

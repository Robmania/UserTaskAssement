using DataBalk.Task.Api.Infrastructure.MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DataBalk.Task.Api.Infrastructure.Helpers;

public static class HelperExtensions
{
    public static IActionResult ToResult(this Result result) => result.IsSuccess ? new OkResult() : new BadRequestObjectResult(result.Failures);

    public static ActionResult ToResult<T>(this Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(result.Failures);

}

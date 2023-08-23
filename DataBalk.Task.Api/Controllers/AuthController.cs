using DataBalk.Task.Api.Data.Models;
using DataBalk.Task.Api.Features.Users.Commands;
using DataBalk.Task.Api.Infrastructure.Services.TokenGenerator;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataBalk.Task.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController: ControllerBase
{
    private readonly ITokenGeneratorService _tokenGeneratorService;
    private readonly IMediator _mediator;

    public AuthController(ITokenGeneratorService tokenGeneratorService, IMediator mediator)
    {
        _tokenGeneratorService = tokenGeneratorService;
        _mediator = mediator;
    }

    /// <summary>
    /// Get Authorisation Token for User
    /// </summary>
    /// <remarks>
    /// <h3>Summary</h3>
    /// <ul>
    /// <li>model validation messages will be returned in 200 response.</li>
    /// </ul>
    /// </remarks>
    /// <response code="200">Returns new token if login was successful</response>
    /// <response code="400">Validation has failed</response>/// 
    /// <param name="login"></param>
    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<IActionResult> CreateToken([FromBody] LoginModel login)
    {
        var ctx = HttpContext.RequestAborted;
        var isValid = await _mediator
            .Send(new IsValidUserCommand() { Username = login.Username, Password = login.Password }, ctx);

        if (isValid.HasFailed || !isValid.Value)
        {
            return Unauthorized();
        }

        var token = _tokenGeneratorService.GenerateToken(login.Username);
        return Ok(new { token });
    }

}

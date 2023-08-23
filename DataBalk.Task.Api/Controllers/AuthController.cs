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

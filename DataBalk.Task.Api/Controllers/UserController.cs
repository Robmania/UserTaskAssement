using AutoMapper;
using DataBalk.Task.Api.Data.Entities;
using DataBalk.Task.Api.Enums;
using DataBalk.Task.Api.Features.Users.Commands;
using DataBalk.Task.Api.Features.Users.Models;
using DataBalk.Task.Api.Features.Users.Queries;
using DataBalk.Task.Api.Infrastructure.Helpers;
using DataBalk.Task.Api.Infrastructure.MediatR;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataBalk.Task.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UserController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Returns a collection of Users
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     Get /api/user
    ///
    /// </remarks>
    /// <returns>List of Users</returns>
    /// <response code="200">Returns all items</response>
    /// <response code="403">Forbidden - Does not have permissions</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsers() => (await _mediator.Send(new GetUsersQuery() { }.WithDetails(Request))).ToResult();

    /// <summary>
    /// Returns a single User by Id
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     Get /api/user
    ///
    /// </remarks>
    /// <returns>List of Users</returns>
    /// <response code="200">Returns all items</response>
    /// <response code="403">Forbidden - Does not have permissions</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="id"></param>
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(long id) => (await _mediator.Send(new GetUserQuery() { Id = id }.WithDetails(Request))).ToResult();

    /// <summary>
    /// Add a new User
    /// </summary>
    /// <remarks>
    /// <h3>Summary</h3>
    /// <ul>
    /// <li>model validation messages will be returned in 200 response.</li>
    /// </ul>
    /// </remarks>
    /// <response code="200">Returns the individual User that was added.</response>
    /// <response code="400">Validation has failed</response>/// 
    /// <response code="403">Forbidden - Does not have permissions</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="model"></param>
    [HttpPost]
    public async Task<ActionResult<User>> Add(UserAddOrUpdateModel model) =>
        (await _mediator.Send(_mapper.Map<AddOrUpdateUserCommand>(model).WithDetails(Request, EOperation.Add))).ToResult();

    /// <summary>
    /// Update an existing User
    /// </summary>
    /// <remarks>
    /// <h3>Summary</h3>
    /// <ul>
    /// <li>model validation messages will be returned in 200 response.</li>
    /// </ul>
    /// </remarks>
    /// <response code="200">Returns the individual User that was updated.</response>
    /// <response code="400">Validation has failed</response>/// 
    /// <response code="403">Forbidden - Does not have permissions</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="model"></param>
    [HttpPut]
    public async Task<ActionResult<User>> Update(UserAddOrUpdateModel model) =>
        (await _mediator.Send(_mapper.Map<AddOrUpdateUserCommand>(model).WithDetails(Request, EOperation.Update))).ToResult();

    /// <summary>
    /// Delete an existing User
    /// </summary>
    /// <remarks>
    /// <h3>Summary</h3>
    /// <ul>
    /// <li>model validation messages will be returned in 200 response.</li>
    /// </ul>
    /// </remarks>
    /// <response code="200">Returns success if User was Deleted.</response>
    /// <response code="403">Forbidden - Does not have permissions</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="id"></param>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(long id) => (await _mediator.Send(new DeleteUserCommand() { Id = id }.WithDetails(Request))).ToResult();
}

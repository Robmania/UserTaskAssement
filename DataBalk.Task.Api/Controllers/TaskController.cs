using AutoMapper;
using DataBalk.Task.Api.Enums;
using DataBalk.Task.Api.Features.Tasks.Commands;
using DataBalk.Task.Api.Features.Tasks.Models;
using DataBalk.Task.Api.Features.Tasks.Queries;
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
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public TaskController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Returns a collection of Tasks
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     Get /api/task
    ///
    /// </remarks>
    /// <returns>List of Tasks</returns>
    /// <response code="200">Returns all items</response>
    /// <response code="403">Forbidden - Does not have permissions</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    public async Task<ActionResult<List<Data.Entities.Task>>> GetTasks() => (await _mediator.Send(new GetTasksQuery() { }.WithDetails(Request))).ToResult();

    /// <summary>
    /// Returns an individual Task based on the id route parameter
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     Get /api/task
    ///
    /// </remarks>
    /// <returns>Returns an individual Task based on the id route parameter</returns>
    /// <response code="200">Returns all items</response>
    /// <response code="403">Forbidden - Does not have permissions</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="id"></param>
    [HttpGet("{id}")]
    public async Task<ActionResult<Data.Entities.Task>> GetTask(long id) => (await _mediator.Send(new GetTaskQuery() { Id = id }.WithDetails(Request))).ToResult();

    /// <summary>
    /// Add a new Task
    /// </summary>
    /// <remarks>
    /// <h3>Summary</h3>
    /// <ul>
    /// <li>model validation messages will be returned in 200 response.</li>
    /// </ul>
    /// </remarks>
    /// <response code="200">Returns the individual Task that was added.</response>
    /// <response code="400">Validation has failed</response>/// 
    /// <response code="403">Forbidden - Does not have permissions</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="model"></param>
    [HttpPost]
    public async Task<ActionResult<Data.Entities.Task>> Add(TaskAddOrUpdateModel model) =>
        (await _mediator.Send(_mapper.Map<AddOrUpdateTaskCommand>(model).WithDetails(Request, EOperation.Add))).ToResult();

    /// <summary>
    /// Update an existing Task
    /// </summary>
    /// <remarks>
    /// <h3>Summary</h3>
    /// <ul>
    /// <li>model validation messages will be returned in 200 response.</li>
    /// </ul>
    /// </remarks>
    /// <response code="200">Returns the individual Task that was updated.</response>
    /// <response code="400">Validation has failed</response>/// 
    /// <response code="403">Forbidden - Does not have permissions</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="model"></param>
    [HttpPut]
    public async Task<ActionResult<Data.Entities.Task>> Update(TaskAddOrUpdateModel model) =>
        (await _mediator.Send(_mapper.Map<AddOrUpdateTaskCommand>(model).WithDetails(Request, EOperation.Update))).ToResult();

    /// <summary>
    /// Delete an existing Task
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
    public async Task<IActionResult> Delete(long id) => (await _mediator.Send(new DeleteTaskCommand() { Id = id }.WithDetails(Request))).ToResult();
}

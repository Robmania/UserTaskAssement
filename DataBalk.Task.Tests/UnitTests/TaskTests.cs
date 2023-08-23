using AutoMapper;
using DataBalk.Task.Api.Constants;
using DataBalk.Task.Api.Data;
using DataBalk.Task.Api.Data.Entities;
using DataBalk.Task.Api.Data.Repositories;
using DataBalk.Task.Api.Enums;
using DataBalk.Task.Api.Features.Tasks.Commands;
using DataBalk.Task.Api.Features.Tasks.Queries;
using DataBalk.Task.Api.Infrastructure.MediatR;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace DataBalk.Task.Tests.UnitTests;
public class TaskTests : TestFixture
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly IMapper _mapper;
    //private readonly Mock<IMapper> _mockMapper;

    private readonly List<Api.Data.Entities.Task> _taskList;
    const string TaskTitle = "test_task_title_001";

    public TaskTests(ITestOutputHelper log) : base(log)
    {
        _mediatorMock = new Mock<IMediator>();
        _mockTaskRepository = new Mock<ITaskRepository>();
        _mockUserRepository = new Mock<IUserRepository>();

        _dbContextMock = new Mock<TaskDbContext>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AddOrUpdateTaskCommandMappingProfile());
        });

        _mapper = mapperConfig.CreateMapper();
       // _mockMapper = new Mock<IMapper>();

        _taskList = new List<Api.Data.Entities.Task>
        {
            new() { Id = 1, Title = TaskTitle }
        };
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTasks_ReturnsOkResult()
    {
        // Arrange
        _mockTaskRepository.Setup(repo => repo.GetAllTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_taskList);

        _mediatorMock.Setup(med => med.Send(It.IsAny<GetTasksQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SuccessResult<List<Api.Data.Entities.Task>>(_taskList));

        var controller = new Api.Controllers.TaskController(_mediatorMock.Object, _mapper);

        // Act
        var result = await controller.GetTasks();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var returnedTasks = (result.Result as OkObjectResult).Value as List<Api.Data.Entities.Task>;
        returnedTasks.Should().HaveCount(1);
        returnedTasks.Should().NotBeNull();
        returnedTasks.First().Title.Should().Be(TaskTitle);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTask_ReturnsOkResult()
    {
        // Arrange
        var returnTask = _taskList.First();

        _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnTask);

        _mediatorMock.Setup(med => med.Send(It.IsAny<GetTaskQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SuccessResult<Api.Data.Entities.Task>(returnTask));

        var controller = new Api.Controllers.TaskController(_mediatorMock.Object, _mapper);

        // Act
        var result = await controller.GetTask(returnTask.Id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var returnedTasks = (result.Result as OkObjectResult).Value as Api.Data.Entities.Task;
        returnedTasks.Title.Should().Be(TaskTitle);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddOrUpdate_Handle_AddOperation_Success()
    {
        // Arrange
        const string title = "Title";
        var task = new Api.Data.Entities.Task { Title = title };

        var cmd = new AddOrUpdateTaskCommand()
        {
            Id = 1,
            Title = title,
            Operation = EOperation.Add,
            Assignee = 1,
            Task = task
        };

        _mockTaskRepository.Setup(repo => repo.CreateTaskAsync(task, It.IsAny<CancellationToken>()))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        var handler = new AddOrUpdateTaskCommandHandler(_mockTaskRepository.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().BeOfType<SuccessResult<Api.Data.Entities.Task>>();
        _mockTaskRepository.Verify(repo => repo.CreateTaskAsync(task, It.IsAny<CancellationToken>()), Times.Once);
        result.Value.Should().BeEquivalentTo(task);
    }

    [Fact]
    public async System.Threading.Tasks.Task Delete_Handle_Delete_Success()
    {
        // Arrange
        var cmd = new DeleteTaskCommand() { Id = 1 };

        _mockTaskRepository.Setup(repo => repo.DeleteTaskAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        var handler = new DeleteTaskCommandHandler(_mockTaskRepository.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().BeOfType<SuccessResult>();
        _mockTaskRepository.Verify(repo => repo.DeleteTaskAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(TestTaskData))]
    public async System.Threading.Tasks.Task AddOrUpdate_Handle_Validation_Error(long assignee, string title, string description, long id, EOperation operation, string expectedError)
    {
        // Arrange
        var task = new Api.Data.Entities.Task { Title = title };
        var user = new User() { Id = assignee };

        var cmd = new AddOrUpdateTaskCommand()
        {
            Id = id,
            Title = title,
            Description = description,
            Operation = operation,
            Assignee = assignee,
            Task = task
        };

        if (id < 2)
        {
            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_taskList.First());
        }
        else
        {
            cmd.Task = null;
        }

        _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var commandValidator = new AddOrUpdateTaskCommandValidator(_mockTaskRepository.Object);

        // Act
        var result = await commandValidator.ValidateAsync(cmd);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(error => error.ErrorMessage.Contains(expectedError));
    }

    public static IEnumerable<object[]> TestTaskData()
    {
        yield return new object[]
        {
            1, string.Empty, "task description", 1, EOperation.Add,
            TaskConstants.ValidationMessages.RequiredField(nameof(AddOrUpdateTaskCommand.Title))
        };
        yield return new object[]
        {
            1, TaskTitle, string.Empty, 1, EOperation.Add,
            TaskConstants.ValidationMessages.RequiredField(nameof(AddOrUpdateTaskCommand.Description))
        };
        yield return new object[]
        {
            1, TaskTitle, "task description", 1, EOperation.Add,
            string.Format(TaskConstants.ValidationMessages.OperationDoesNotMatchId, EOperation.Add)
        };
        yield return new object[]
        {
            1, TaskTitle, "task description", 0, EOperation.Update,
            string.Format(TaskConstants.ValidationMessages.OperationDoesNotMatchId, EOperation.Update)
        };
        yield return new object[]
        {
            1, TaskTitle, "task description", 3, EOperation.Update,
            string.Format(TaskConstants.ValidationMessages.DoesNotExist, "Task", 3)
        };
    }

}

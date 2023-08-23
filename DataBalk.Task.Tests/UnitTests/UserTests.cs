using AutoMapper;
using DataBalk.Task.Api.Data.Repositories;
using MediatR;
using Moq;
using DataBalk.Task.Api.Data.Entities;
using Xunit.Abstractions;
using DataBalk.Task.Api.Data;
using DataBalk.Task.Api.Features.Users.Queries;
using DataBalk.Task.Api.Infrastructure.MediatR;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using DataBalk.Task.Api.Features.Tasks.Commands;
using DataBalk.Task.Api.Features.Users.Commands;
using Microsoft.Extensions.Logging;
using DataBalk.Task.Api.Enums;

namespace DataBalk.Task.Tests.UnitTests;
public class UserTests : TestFixture
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IUserRepository> _mockUserRepository;

    private readonly IMapper _mapper;
    private readonly List<User> _userList;

    const string UserName = "user_name_001";
    public UserTests(ITestOutputHelper log) : base(log)
    {
        _mediatorMock = new Mock<IMediator>();
        _mockUserRepository = new Mock<IUserRepository>();

        _dbContextMock = new Mock<TaskDbContext>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AddOrUpdateUserCommandMappingProfile());
        });

        _mapper = mapperConfig.CreateMapper();

        _userList = new List<User>()
        {
            new() { Id= 1, Username = UserName}
        };
    }

    [Fact]
    public async System.Threading.Tasks.Task GetUsers_ReturnsOkResult()
    {
        // Arrange
        _mockUserRepository.Setup(repo => repo.GetAllUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_userList);

        _mediatorMock.Setup(med => med.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SuccessResult<List<User>>(_userList));

        var controller = new Api.Controllers.UserController(_mediatorMock.Object, _mapper);

        // Act
        var result = await controller.GetUsers();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var returnedTasks = (result.Result as OkObjectResult).Value as List<User>;
        returnedTasks.Should().HaveCount(1);
        returnedTasks.Should().NotBeNull();
        returnedTasks.First().Username.Should().Be(UserName);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetUser_ReturnsOkResult()
    {
        // Arrange
        var returnUser = _userList.First();

        _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnUser);

        _mediatorMock.Setup(med => med.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SuccessResult<User>(returnUser));

        var controller = new Api.Controllers.UserController(_mediatorMock.Object, _mapper);

        // Act
        var result = await controller.GetUser(returnUser.Id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var returnedUser = (result.Result as OkObjectResult).Value as User;
        returnedUser.Should().NotBeNull();
        returnedUser.Username.Should().Be(UserName);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddOrUpdate_Handle_AddOperation_Success()
    {
        // Arrange
        const string username = "Username";
        var user = new User() { Username = username };

        var cmd = new AddOrUpdateUserCommand()
        {
            Id = 1,
            Username = username,
            Password = "TestPassword@01",
            Operation = EOperation.Add,
            EMail = "test@test.co.za",
            User = user
        };

        _mockUserRepository.Setup(repo => repo.CreateUserAsync(user, It.IsAny<CancellationToken>()))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        var handler = new AddOrUpdateUserCommandHandler(_mockUserRepository.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().BeOfType<SuccessResult<User>>();
        _mockUserRepository.Verify(repo => repo.CreateUserAsync(user,It.IsAny<CancellationToken>()), Times.Once);
        result.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async System.Threading.Tasks.Task Delete_Handle_Delete_Success()
    {
        // Arrange
        var cmd = new DeleteUserCommand() { Id = 1 };

        _mockUserRepository.Setup(repo => repo.DeleteUserAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        var handler = new DeleteUserCommandHandler(_mockUserRepository.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().BeOfType<SuccessResult>();
        _mockUserRepository.Verify(repo => repo.DeleteUserAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

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
using DataBalk.Task.Api.Features.Users.Commands;
using DataBalk.Task.Api.Enums;
using static DataBalk.Task.Api.Constants.TaskConstants;

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

    [Theory]
    [MemberData(nameof(TestUserData))]
    public async System.Threading.Tasks.Task AddOrUpdate_Handle_Validation_Error(long id, string username, string password, string email, EOperation operation, string expectedError)
    {        
        // Arrange
        var user = new User() { Username = username };

        var cmd = new AddOrUpdateUserCommand()
        {
            Id = id,
            Username = username,
            Password = password,
            Operation = operation,
            EMail = email,
            User = user
        };

        if (id < 2)
        {
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_userList.First);
        }
        else
        {
            cmd.User = null;
        }

        var commandValidator = new AddOrUpdateUserCommandValidator(_mockUserRepository.Object);

        // Act
        var result = await commandValidator.ValidateAsync(cmd);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(error => error.ErrorMessage.Contains(expectedError));
    }

    public static IEnumerable<object[]> TestUserData()
    {
        yield return new object[]
        {
            0,"Username", "TestPassword@01", "test@test.co.za", EOperation.Update,
            string.Format(ValidationMessages.OperationDoesNotMatchId, EOperation.Update)
        };
        yield return new object[]
        {
            1,"Username", "TestPassword@01", "test@test.co.za", EOperation.Add,
            string.Format(ValidationMessages.OperationDoesNotMatchId, EOperation.Add)
        };
        yield return new object[]
        {
            0,string.Empty, "TestPassword@01", "test@test.co.za", EOperation.Add,
            ValidationMessages.RequiredField(nameof(AddOrUpdateUserCommand.Username))
        };
        yield return new object[]
        {
            0,"Username", "TestPassword@01", "invalid_email", EOperation.Add,
            "A valid email is required"
        };
        yield return new object[]
        {
            0,"Username", "TestPassword@01", string.Empty, EOperation.Add,
            ValidationMessages.RequiredField(nameof(AddOrUpdateUserCommand.EMail))
        };
        yield return new object[]
        {
            0,"Username",string.Empty, "test@test.co.za", EOperation.Add,
            ValidationMessages.RequiredField(nameof(AddOrUpdateUserCommand.Password))
        };
        yield return new object[]
        {
            0,"Username",string.Empty, "test@test.co.za", EOperation.Add,
            ValidationMessages.RequiredField(nameof(AddOrUpdateUserCommand.Password))
        };
        yield return new object[]
        {
            0,"Username","short", "test@test.co.za", EOperation.Add,
            "Your password length must be at least 8."
        };
        yield return new object[]
        {
            0,"Username","very-long-password", "test@test.co.za", EOperation.Add,
            "Your password length must not exceed 16."
        };
        yield return new object[]
        {
            0,"Username","lowercasepwd", "test@test.co.za", EOperation.Add,
            "Your password must contain at least one uppercase letter."
        };
        yield return new object[]
        {
            0,"Username","UPPERCASEPWD", "test@test.co.za", EOperation.Add,
            "Your password must contain at least one lowercase letter."
        };
        yield return new object[]
        {
            0,"Username","noSpecialChars", "test@test.co.za", EOperation.Add,
            "Your password must contain at least one (!@#$%^&*)."
        };
    }
}

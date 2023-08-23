using DataBalk.Task.Api.Data.Entities;
using DataBalk.Task.Api.Infrastructure.MediatR;
using MediatR;
using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using DataBalk.Task.Api.Infrastructure.Validators;
using static DataBalk.Task.Api.Constants.TaskConstants;
using DataBalk.Task.Api.Enums;
using Microsoft.EntityFrameworkCore;
using DataBalk.Task.Api.Features.Users.Models;
using DataBalk.Task.Api.Data.Repositories;

namespace DataBalk.Task.Api.Features.Users.Commands;

public class AddOrUpdateUserCommand : AppCommand, IRequest<Result<User>>
{
    public string Username { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public override string CommandDescription => "Add or Update a User";

    [JsonIgnore]
    public User? User { get; set; }
}

public class AddOrUpdateUserCommandValidator : AbstractValidator<AddOrUpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public AddOrUpdateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        RuleFor(cmd => cmd).OperationMustMatchId(cmd => cmd.Id);
        RuleFor(cmd => cmd).MustAsync(UserMustExistInDb)
            .WithMessage(cmd => string.Format(ValidationMessages.DoesNotExist, "User", cmd.Id))
            .DependentRules(
                () =>
                {
                    RuleFor(qry => qry.Username).NotEmpty()
                        .WithMessage(ValidationMessages.RequiredField(nameof(AddOrUpdateUserCommand.Username)))
                        .DependentRules(
                            () =>
                            {
                                RuleFor(cmd => cmd).MustAsync(UsernameMustNotExistInDb)
                                    .WithMessage($"A user with this username already exists.")
                                    .DependentRules(
                                        () =>
                                        {
                                            RuleFor(qry => qry.EMail).NotEmpty()
                                                .WithMessage(ValidationMessages.RequiredField(nameof(AddOrUpdateUserCommand.EMail)))
                                                .EmailAddress().WithMessage("A valid email is required");
                                            RuleFor(qry => qry.Password).NotEmpty().WithMessage(ValidationMessages.RequiredField(nameof(AddOrUpdateUserCommand.Password)))
                                                                        .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                                                                        .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                                                                        .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                                                                        .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                                                                        .Matches(@"[\!\@\#\$\%\^\&\*]+").WithMessage("Your password must contain at least one (!@#$%^&*).");
                                        });
                            });

                });
    }

    private async Task<bool> UserMustExistInDb(AddOrUpdateUserCommand cmd, CancellationToken ctx)
    {
        cmd.User = cmd.Operation == EOperation.Add ? new User() : await _userRepository.GetUserByIdAsync(cmd.Id, ctx);

        return cmd.User != null;
    }

    private async Task<bool> UsernameMustNotExistInDb(AddOrUpdateUserCommand cmd, CancellationToken ctx)
    {
        var usernameAlreadyExists = false;

        if (cmd.Operation == EOperation.Add)
        {
            usernameAlreadyExists =
                await _userRepository.UsernameAlreadyExistsAsync(cmd.Username, ctx);
        }

        return !usernameAlreadyExists;
    }
}

public class AddOrUpdateUserCommandHandler : IRequestHandler<AddOrUpdateUserCommand, Result<User>>
{
    private readonly IUserRepository _userRepository;

    public AddOrUpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<User>> Handle(AddOrUpdateUserCommand cmd, CancellationToken ctx)
    {
        cmd.User.Username = cmd.Username.ToLower();
        cmd.User.EMail = cmd.EMail;

        if (cmd.Operation == EOperation.Add)
        {
            cmd.User.Password = cmd.Password;
            await _userRepository.CreateUserAsync(cmd.User, ctx);
        }
        else
        {
            await _userRepository.UpdateUserAsync(cmd.User, ctx);
        }
        
        return new SuccessResult<User>(cmd.User);
    }
}

public class AddOrUpdateUserCommandMappingProfile : Profile
{
    public AddOrUpdateUserCommandMappingProfile()
    {
        CreateMap<UserAddOrUpdateModel, AddOrUpdateUserCommand>();
    }
}


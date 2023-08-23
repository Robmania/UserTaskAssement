using DataBalk.Task.Api.Constants;
using DataBalk.Task.Api.Data.Repositories;
using DataBalk.Task.Api.Infrastructure.MediatR;
using FluentValidation;
using MediatR;

namespace DataBalk.Task.Api.Features.Users.Commands;

public class IsValidUserCommand : AppCommand, IRequest<Result<bool>>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public override string CommandDescription => "Valid user";
}

public class IsValidUserCommandValidator : AbstractValidator<IsValidUserCommand>
{
    public IsValidUserCommandValidator()
    {
        RuleFor(cmd => cmd.Username).NotEmpty()
            .WithMessage(TaskConstants.ValidationMessages.RequiredField(nameof(IsValidUserCommand.Username)));
        RuleFor(cmd => cmd.Password).NotEmpty()
            .WithMessage(TaskConstants.ValidationMessages.RequiredField(nameof(IsValidUserCommand.Password)));
    }
}

public class IsValidUserCommandHandler: IRequestHandler<IsValidUserCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;

    public IsValidUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> Handle(IsValidUserCommand cmd, CancellationToken ctx)
    {
        var isValid = await _userRepository.IsUserValidAsync(cmd.Username.ToLower(), cmd.Password, ctx);

        return new SuccessResult<bool>(isValid);
    }
}

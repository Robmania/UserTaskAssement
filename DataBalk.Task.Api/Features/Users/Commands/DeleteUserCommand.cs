using DataBalk.Task.Api.Constants;
using DataBalk.Task.Api.Data.Repositories;
using DataBalk.Task.Api.Infrastructure.MediatR;
using FluentValidation;
using MediatR;

namespace DataBalk.Task.Api.Features.Users.Commands;

public class DeleteUserCommand: AppCommand, IRequest<Result>
{
    public override string CommandDescription => "Delete an existing User";
}

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(qry => qry.Id)
            .NotEmpty()
            .WithMessage(TaskConstants.ValidationMessages.RequiredField(nameof(DeleteUserCommand.Id)));
    }
}

public class DeleteUserCommandHandler: IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    public async Task<Result> Handle(DeleteUserCommand cmd, CancellationToken ctx)
    {
        await _userRepository.DeleteUserAsync(cmd.Id, ctx);

        return new SuccessResult();
    }
}

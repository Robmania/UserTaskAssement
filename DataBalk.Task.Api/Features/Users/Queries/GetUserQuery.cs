using DataBalk.Task.Api.Constants;
using DataBalk.Task.Api.Data.Entities;
using DataBalk.Task.Api.Data.Repositories;
using DataBalk.Task.Api.Infrastructure.MediatR;
using FluentValidation;
using MediatR;

namespace DataBalk.Task.Api.Features.Users.Queries;

public class GetUserQuery : AppMessage, IRequest<Result<User>>
{
    public long Id { get; set; }
}

public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        RuleFor(qry => qry.Id).NotEmpty().WithMessage(TaskConstants.ValidationMessages.RequiredField(nameof(GetUserQuery.Id)));
    }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<User>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserQuery> _logger;

    public GetUserQueryHandler(ILogger<GetUserQuery> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<Result<User>> Handle(GetUserQuery qry, CancellationToken ctx)
    {
        _logger.LogInformation($"{nameof(GetUserQuery)} start");

        var user = await _userRepository.GetUserByIdAsync(qry.Id, ctx);

        return new SuccessResult<User>(user);
    }
}

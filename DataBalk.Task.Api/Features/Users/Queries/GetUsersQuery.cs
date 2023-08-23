using DataBalk.Task.Api.Data.Entities;
using DataBalk.Task.Api.Data.Repositories;
using DataBalk.Task.Api.Infrastructure.MediatR;
using MediatR;

namespace DataBalk.Task.Api.Features.Users.Queries;

public class GetUsersQuery : AppMessage, IRequest<Result<List<User>>>
{
}

public class GetUsersQueryHandler: IRequestHandler<GetUsersQuery, Result<List<User>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUsersQuery> _logger;

    public GetUsersQueryHandler(ILogger<GetUsersQuery> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<Result<List<User>>> Handle(GetUsersQuery qry, CancellationToken ctx)
    {
        _logger.LogInformation($"{nameof(GetUsersQuery)} start");

        var users = await _userRepository.GetAllUsersAsync(ctx);

        return new SuccessResult<List<User>>(users.OrderBy(u => u.Username).ToList());
    }
}

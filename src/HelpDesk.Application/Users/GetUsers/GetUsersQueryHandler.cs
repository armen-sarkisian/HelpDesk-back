using HelpDesk.Application.Abstractions.Persistence;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Users.GetUsers;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository users, IMapper mapper)
    {
        _users = users;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _users.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<UserDto>>(users);
    }
}

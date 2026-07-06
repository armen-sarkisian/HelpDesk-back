using MediatR;

namespace HelpDesk.Application.Users.GetUsers;

/// <summary>Lists all users (e.g. for an admin choosing an assignee). No parameters, no validator.</summary>
public sealed record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;

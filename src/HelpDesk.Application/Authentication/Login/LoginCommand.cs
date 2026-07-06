using MediatR;

namespace HelpDesk.Application.Authentication.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

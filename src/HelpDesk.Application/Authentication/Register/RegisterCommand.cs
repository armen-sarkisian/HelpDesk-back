using MediatR;

namespace HelpDesk.Application.Authentication.Register;

public sealed record RegisterCommand(string Name, string Email, string Password) : IRequest<AuthResponse>;

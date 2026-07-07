using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Authentication;

/// <summary>Returned by register/login: the access token plus the basic identity the client shows.</summary>
public sealed record AuthResponse(string Token, Guid UserId, string Name, string Email, UserRole Role);

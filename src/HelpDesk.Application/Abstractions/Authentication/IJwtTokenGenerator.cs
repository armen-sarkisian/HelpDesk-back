using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Abstractions.Authentication;

/// <summary>
/// Issues a signed JWT access token for an authenticated user. Signing keys and lifetime
/// are infrastructure concerns hidden behind this contract.
/// </summary>
public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}

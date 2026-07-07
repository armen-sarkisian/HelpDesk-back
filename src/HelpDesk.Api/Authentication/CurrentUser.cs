using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Api.Authentication;

/// <summary>
/// Reads the caller's identity from the authenticated <c>HttpContext</c>. This is the single
/// place that knows how claims are laid out, so the Application layer stays free of web types.
/// </summary>
public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public Guid Id =>
        Guid.TryParse(Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id)
            ? id
            : Guid.Empty;

    public bool IsAdmin => Principal?.IsInRole(nameof(UserRole.Admin)) ?? false;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;
}

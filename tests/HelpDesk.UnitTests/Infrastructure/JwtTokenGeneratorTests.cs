using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Authentication;
using Microsoft.Extensions.Options;

namespace HelpDesk.UnitTests.Infrastructure;

public class JwtTokenGeneratorTests
{
    private static readonly JwtSettings Settings = new()
    {
        Issuer = "HelpDesk.Api",
        Audience = "HelpDesk.Client",
        Key = "test-signing-key-that-is-long-enough-for-hmacsha256-0123456789",
        ExpiryMinutes = 60
    };

    private readonly JwtTokenGenerator _generator = new(Options.Create(Settings));

    [Fact]
    public void GenerateToken_embeds_user_identity_and_role_claims()
    {
        var user = new User("Alice", "alice@example.com", "hash", UserRole.Admin);

        var token = _generator.GenerateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(Settings.Issuer, jwt.Issuer);
        Assert.Contains(Settings.Audience, jwt.Audiences);
        Assert.Equal(user.Id.ToString(), jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(user.Email, jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal(nameof(UserRole.Admin), jwt.Claims.Single(c => c.Type == ClaimTypes.Role).Value);
    }

    [Fact]
    public void GenerateToken_sets_an_expiry_in_the_future()
    {
        var user = new User("Bob", "bob@example.com", "hash", UserRole.User);

        var token = _generator.GenerateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }
}

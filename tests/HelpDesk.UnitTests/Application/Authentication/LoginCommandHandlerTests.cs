using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Authentication.Login;
using HelpDesk.Application.Common.Exceptions;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Authentication;
using NSubstitute;

namespace HelpDesk.UnitTests.Application.Authentication;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = new Pbkdf2PasswordHasher();
    private readonly IJwtTokenGenerator _tokenGenerator = Substitute.For<IJwtTokenGenerator>();

    private LoginCommandHandler CreateHandler() =>
        new(_users, _passwordHasher, _tokenGenerator);

    private User UserWithPassword(string password) =>
        new("Alice", "alice@example.com", _passwordHasher.Hash(password), UserRole.User);

    [Fact]
    public async Task Handle_throws_unauthorized_when_user_is_not_found()
    {
        _users.GetByEmailAsync("ghost@example.com", Arg.Any<CancellationToken>()).Returns((User?)null);

        var handler = CreateHandler();
        var command = new LoginCommand("ghost@example.com", "whatever");

        await Assert.ThrowsAsync<UnauthorizedException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_throws_unauthorized_when_password_is_wrong()
    {
        _users.GetByEmailAsync("alice@example.com", Arg.Any<CancellationToken>())
            .Returns(UserWithPassword("correct-password"));

        var handler = CreateHandler();
        var command = new LoginCommand("alice@example.com", "wrong-password");

        await Assert.ThrowsAsync<UnauthorizedException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_returns_a_token_for_valid_credentials()
    {
        _users.GetByEmailAsync("alice@example.com", Arg.Any<CancellationToken>())
            .Returns(UserWithPassword("correct-password"));
        _tokenGenerator.GenerateToken(Arg.Any<User>()).Returns("issued-token");

        var handler = CreateHandler();
        var command = new LoginCommand("Alice@Example.com", "correct-password");

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("issued-token", response.Token);
        Assert.Equal("alice@example.com", response.Email);
    }
}

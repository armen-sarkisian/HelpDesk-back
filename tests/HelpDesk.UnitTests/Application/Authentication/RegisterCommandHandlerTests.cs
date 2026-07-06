using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Authentication.Register;
using HelpDesk.Application.Common.Exceptions;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure.Authentication;
using NSubstitute;

namespace HelpDesk.UnitTests.Application.Authentication;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasher _passwordHasher = new Pbkdf2PasswordHasher();
    private readonly IJwtTokenGenerator _tokenGenerator = Substitute.For<IJwtTokenGenerator>();

    private RegisterCommandHandler CreateHandler() =>
        new(_users, _unitOfWork, _passwordHasher, _tokenGenerator);

    [Fact]
    public async Task Handle_throws_conflict_when_email_already_registered()
    {
        _users.EmailExistsAsync("taken@example.com", Arg.Any<CancellationToken>()).Returns(true);

        var handler = CreateHandler();
        var command = new RegisterCommand("Taken", "taken@example.com", "password123");

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(command, CancellationToken.None));
        await _users.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_persists_a_user_with_a_normalised_email_and_hashed_password()
    {
        _users.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _tokenGenerator.GenerateToken(Arg.Any<User>()).Returns("issued-token");

        User? saved = null;
        await _users.AddAsync(Arg.Do<User>(u => saved = u), Arg.Any<CancellationToken>());

        var handler = CreateHandler();
        var command = new RegisterCommand("  New User ", "New@Example.com", "password123");

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(saved);
        Assert.Equal("New User", saved!.Name);
        Assert.Equal("new@example.com", saved.Email);          // trimmed + lower-cased
        Assert.Equal(UserRole.User, saved.Role);                // self-registration is never admin
        Assert.NotEqual("password123", saved.PasswordHash);     // stored hashed
        Assert.True(_passwordHasher.Verify("password123", saved.PasswordHash));

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        Assert.Equal("issued-token", response.Token);
        Assert.Equal("new@example.com", response.Email);
    }
}

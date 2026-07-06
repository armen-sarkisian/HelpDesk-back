using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Common.Exceptions;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using MediatR;

namespace HelpDesk.Application.Authentication.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public RegisterCommandHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Normalise the email so uniqueness doesn't hinge on casing/whitespace.
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _users.EmailExistsAsync(email, cancellationToken))
            throw new ConflictException($"Email '{email}' is already registered.");

        // Self-registration always creates a plain User; admins are provisioned via seeding.
        var user = new User(
            request.Name.Trim(),
            email,
            _passwordHasher.Hash(request.Password),
            UserRole.User);

        await _users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenGenerator.GenerateToken(user);
        return new AuthResponse(token, user.Id, user.Name, user.Email, user.Role);
    }
}

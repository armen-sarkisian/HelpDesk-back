using FluentValidation;

namespace HelpDesk.Application.Authentication.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        // Only shape checks here — whether the credentials are correct is decided by the handler.
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

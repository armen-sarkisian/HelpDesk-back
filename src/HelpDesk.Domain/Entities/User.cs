using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

/// <summary>
/// An account in the system. The domain stores only the password *hash* — hashing itself
/// is an infrastructure concern, so the entity never sees raw passwords.
/// </summary>
public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }

    // Required by EF Core's materialization; not for application use.
    private User()
    {
    }

    public User(string name, string email, string passwordHash, UserRole role = UserRole.User)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public bool IsAdmin => Role == UserRole.Admin;
}

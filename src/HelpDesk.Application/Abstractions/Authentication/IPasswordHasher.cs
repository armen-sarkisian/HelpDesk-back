namespace HelpDesk.Application.Abstractions.Authentication;

/// <summary>
/// Hashes and verifies passwords. The algorithm is an infrastructure detail; the Application
/// layer only depends on this contract so handlers never see raw crypto.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}

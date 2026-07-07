using System.Security.Cryptography;
using HelpDesk.Application.Abstractions.Authentication;

namespace HelpDesk.Infrastructure.Authentication;

/// <summary>
/// PBKDF2 (HMAC-SHA256) password hasher. Uses a per-password random salt and a high iteration
/// count, and compares in constant time. We rely on the framework's vetted PBKDF2 primitive
/// rather than inventing a scheme — "don't roll your own crypto". The stored value is
/// self-describing (<c>iterations;salt;hash</c>) so parameters can evolve without breaking
/// existing hashes.
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;          // 128-bit salt
    private const int KeySize = 32;           // 256-bit derived key
    private const int Iterations = 100_000;
    private const char Delimiter = ';';
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return string.Join(Delimiter,
            Iterations,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool Verify(string password, string passwordHash)
    {
        var parts = passwordHash.Split(Delimiter);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var expectedHash = Convert.FromBase64String(parts[2]);

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}

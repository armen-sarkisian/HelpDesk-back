using HelpDesk.Infrastructure.Authentication;

namespace HelpDesk.UnitTests.Infrastructure;

public class Pbkdf2PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void Verify_returns_true_for_the_correct_password()
    {
        var hash = _hasher.Hash("Sup3rSecret!");

        Assert.True(_hasher.Verify("Sup3rSecret!", hash));
    }

    [Fact]
    public void Verify_returns_false_for_a_wrong_password()
    {
        var hash = _hasher.Hash("Sup3rSecret!");

        Assert.False(_hasher.Verify("wrong-password", hash));
    }

    [Fact]
    public void Hash_never_returns_the_plaintext()
    {
        const string password = "Sup3rSecret!";

        var hash = _hasher.Hash(password);

        Assert.DoesNotContain(password, hash);
    }

    [Fact]
    public void Hashing_the_same_password_twice_yields_different_hashes()
    {
        // A random per-password salt means identical passwords must not produce identical hashes.
        var first = _hasher.Hash("Sup3rSecret!");
        var second = _hasher.Hash("Sup3rSecret!");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Verify_returns_false_for_a_malformed_hash()
    {
        Assert.False(_hasher.Verify("whatever", "not-a-valid-hash"));
    }
}

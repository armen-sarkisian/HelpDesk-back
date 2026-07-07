namespace HelpDesk.Infrastructure.Authentication;

/// <summary>
/// Strongly-typed JWT configuration, bound from the "Jwt" section. The signing <see cref="Key"/>
/// is a secret — it belongs in user-secrets / environment variables in real environments, not in
/// source control.
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}

using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Users;

/// <summary>
/// Outward-facing view of a user. Deliberately omits <c>PasswordHash</c> — DTOs are also a
/// security boundary, not just a mapping convenience. Mapster maps <see cref="Domain.Entities.User"/>
/// to this by matching property names, so no explicit config is needed.
/// </summary>
public sealed record UserDto(Guid Id, string Name, string Email, UserRole Role);

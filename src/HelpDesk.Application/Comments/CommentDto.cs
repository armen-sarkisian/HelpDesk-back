namespace HelpDesk.Application.Comments;

/// <summary>Outward view of a comment, including its author's display name.</summary>
public sealed record CommentDto(Guid Id, string Message, DateTime CreatedAt, Guid UserId, string UserName);

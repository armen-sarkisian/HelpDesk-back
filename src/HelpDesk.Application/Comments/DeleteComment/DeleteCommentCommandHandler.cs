using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Common.Exceptions;
using MediatR;

namespace HelpDesk.Application.Comments.DeleteComment;

public sealed class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly ICommentRepository _comments;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommentCommandHandler(ICommentRepository comments, IUnitOfWork unitOfWork)
    {
        _comments = comments;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _comments.GetByIdAsync(request.CommentId, cancellationToken)
            ?? throw NotFoundException.For("Comment", request.CommentId);

        _comments.Remove(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

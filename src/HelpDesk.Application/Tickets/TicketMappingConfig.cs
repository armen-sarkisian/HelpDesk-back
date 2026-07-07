using HelpDesk.Application.Comments;
using HelpDesk.Domain.Entities;
using Mapster;

namespace HelpDesk.Application.Tickets;

/// <summary>
/// Explicit Mapster mappings for tickets. The author/assignee display names are flattened from
/// navigation properties, so callers must load <c>CreatedBy</c>/<c>AssignedTo</c> before mapping.
/// Being explicit (rather than relying purely on convention) makes the null-safe assignee mapping
/// obvious and guards against silent breakage if a property is renamed.
/// </summary>
public sealed class TicketMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Ticket, TicketDto>()
            .Map(dest => dest.CreatedByName, src => src.CreatedBy.Name)
            .Map(dest => dest.AssignedToName, src => src.AssignedTo != null ? src.AssignedTo.Name : null);

        config.NewConfig<Ticket, TicketDetailsDto>()
            .Map(dest => dest.CreatedByName, src => src.CreatedBy.Name)
            .Map(dest => dest.AssignedToName, src => src.AssignedTo != null ? src.AssignedTo.Name : null)
            .Map(dest => dest.Comments, src => src.Comments);

        config.NewConfig<Comment, CommentDto>()
            .Map(dest => dest.UserName, src => src.User.Name);
    }
}

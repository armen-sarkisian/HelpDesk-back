using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Message).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.CreatedAt).IsRequired();

        // Author of the comment. Restrict so removing a user doesn't silently wipe history;
        // the Ticket → Comments cascade already covers the "delete ticket" case, and keeping
        // this side non-cascading avoids SQL Server's multiple-cascade-path error.
        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.TicketId);
    }
}

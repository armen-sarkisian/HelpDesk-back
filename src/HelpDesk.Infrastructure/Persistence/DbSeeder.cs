using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence;

/// <summary>
/// Seeds a fresh database with one admin, a few users, tickets and comments so the app is usable
/// out of the box. Idempotent: it bails out if any users already exist. Statuses are advanced
/// through the domain's <see cref="Ticket.ChangeStatus"/> so seed data respects the same workflow
/// invariants as the running app.
/// </summary>
public static class DbSeeder
{
    private const string defaultPassword = "Password123!";
    private const string adminPassword = "Admin123!";

    public static async Task SeedAsync(HelpDeskDbContext context, IPasswordHasher passwordHasher, CancellationToken cancellationToken = default)
    {
        if (await context.Users.AnyAsync(cancellationToken))
            return;

        var admin = new User("Admin", "admin@helpdesk.local", passwordHasher.Hash(adminPassword), UserRole.Admin);
        var alice = new User("Alice Johnson", "alice@helpdesk.local", passwordHasher.Hash(defaultPassword));
        var bob = new User("Bob Smith", "bob@helpdesk.local", passwordHasher.Hash(defaultPassword));
        var carol = new User("Carol White", "carol@helpdesk.local", passwordHasher.Hash(defaultPassword));

        await context.Users.AddRangeAsync([admin, alice, bob, carol], cancellationToken);

        var loginIssue = new Ticket(
            "Cannot log in to the portal",
            "I get 'invalid credentials' even though my password is correct.",
            TicketPriority.High,
            alice.Id);

        var printerIssue = new Ticket(
            "Office printer not responding",
            "The 3rd-floor printer shows offline for everyone.",
            TicketPriority.Medium,
            bob.Id);
        printerIssue.ChangeStatus(TicketStatus.InProgress);
        printerIssue.Assign(admin.Id);

        var darkMode = new Ticket(
            "Feature request: dark mode",
            "Please add a dark theme to reduce eye strain.",
            TicketPriority.Low,
            carol.Id);
        darkMode.ChangeStatus(TicketStatus.InProgress);
        darkMode.ChangeStatus(TicketStatus.Resolved);
        darkMode.Assign(admin.Id);

        var emailDelay = new Ticket(
            "Email sync is delayed",
            "Incoming email takes ~30 minutes to appear.",
            TicketPriority.Critical,
            alice.Id);

        await context.Tickets.AddRangeAsync([loginIssue, printerIssue, darkMode, emailDelay], cancellationToken);

        await context.Comments.AddRangeAsync(
        [
            new Comment(loginIssue.Id, admin.Id, "Thanks for reporting — we're looking into it."),
            new Comment(loginIssue.Id, alice.Id, "Appreciated, let me know if you need more details."),
            new Comment(printerIssue.Id, admin.Id, "Assigned to me, investigating the print server."),
            new Comment(darkMode.Id, admin.Id, "Shipped in the latest release. Marking as resolved.")
        ], cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}

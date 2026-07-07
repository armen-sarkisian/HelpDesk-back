using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Infrastructure.Authentication;
using HelpDesk.Infrastructure.Persistence;
using HelpDesk.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer. The API calls this to register EF Core,
/// repositories, JWT and other external services so the wiring lives next to the
/// implementations instead of leaking into <c>Program.cs</c>. It reads its own settings from
/// configuration (connection string + "Jwt" section).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<HelpDeskDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories and the unit of work share the scoped DbContext, so one SaveChanges
        // commits everything staged during a single request.
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        // Authentication services.
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HelpDesk.Infrastructure.Persistence;

/// <summary>
/// Lets the EF Core tools (<c>dotnet ef migrations/database</c>) build the context without
/// spinning up the API host. The connection string here is only used at design time; the
/// running application supplies its own via <c>AddInfrastructure</c>. Override with the
/// <c>HELPDESK_DESIGN_CONNECTION</c> environment variable if your LocalDB name differs.
/// </summary>
public sealed class HelpDeskDbContextFactory : IDesignTimeDbContextFactory<HelpDeskDbContext>
{
    private const string DefaultDesignConnection =
        "Server=localhost;Database=HelpDesk;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public HelpDeskDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("HELPDESK_DESIGN_CONNECTION") ?? DefaultDesignConnection;

        var options = new DbContextOptionsBuilder<HelpDeskDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new HelpDeskDbContext(options);
    }
}

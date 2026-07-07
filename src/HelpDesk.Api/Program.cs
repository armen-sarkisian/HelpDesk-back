using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HelpDesk.Api.Authentication;
using HelpDesk.Api.Authorization;
using HelpDesk.Api.Common;
using HelpDesk.Api.Realtime;
using HelpDesk.Application;
using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Notifications;
using HelpDesk.Domain.Enums;
using HelpDesk.Infrastructure;
using HelpDesk.Infrastructure.Authentication;
using HelpDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog is configured from appsettings so log levels/sinks are ops-tunable without a rebuild.
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// MVC controllers handle the HTTP surface.
builder.Services.AddControllers();

// Application + Infrastructure composition roots (CQRS, EF, repositories, auth services).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Expose the caller's identity to handlers.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddScoped<ExceptionHandlingMiddleware>();

// Real-time notifications.
builder.Services.AddSignalR();
builder.Services.AddScoped<ITicketNotifier, TicketNotifier>();

// OpenAPI document (NSwag) — the source for Swagger UI and the generated TypeScript client.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "HelpDesk API";
    settings.Version = "v1";
    settings.AddSecurity("Bearer", new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste the JWT access token (without the 'Bearer ' prefix)."
    });
    settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});

// JWT authentication: validate issuer, audience, signing key and lifetime on every request.
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("Missing 'Jwt' configuration section.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Keep claim types exactly as issued (don't remap "sub" -> nameidentifier).
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = JwtRegisteredClaimNames.Sub
        };

        // WebSockets can't send an Authorization header, so accept the token from the query
        // string for the SignalR hub path.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) &&
                    context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.Admin, policy => policy.RequireRole(nameof(UserRole.Admin)));

var app = builder.Build();

// Error translation wraps everything below it.
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();

// API docs (dev only): OpenAPI JSON at /swagger/v1/swagger.json, UI at /swagger.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<TicketsHub>("/hubs/tickets").RequireAuthorization();

// Apply pending migrations and seed on startup so a fresh clone is runnable with no manual steps.
await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HelpDeskDbContext>();
    await context.Database.MigrateAsync();

    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DbSeeder.SeedAsync(context, passwordHasher);
}

app.Run();

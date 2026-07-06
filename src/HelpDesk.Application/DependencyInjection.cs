using System.Globalization;
using System.Reflection;
using FluentValidation;
using HelpDesk.Application.Common.Behaviors;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.Application;

/// <summary>
/// Composition root for the Application layer: MediatR (CQRS), FluentValidation and Mapster.
/// Everything is discovered by scanning this assembly, so adding a new handler / validator /
/// mapping needs no change here.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Keep validation messages in English regardless of the server's locale, so the API
        // contract is deterministic.
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

        // CQRS: handlers are auto-registered from the assembly.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Validation runs as the first pipeline step for every request that has a validator.
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Mapping: pick up any IRegister configs, then expose a DI-aware IMapper.
        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        mapsterConfig.Scan(assembly);
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}

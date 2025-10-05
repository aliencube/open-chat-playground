using System.Reflection;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace OpenChat.PlaygroundApp.Endpoints;

/// <summary>
/// This represents the extension entity for handling endpoints.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Adds the chat client to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
    /// <param name="assembly">The <see cref="Assembly"/> instance.</param>
    /// <returns>Returns the modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var descriptors = assembly.DefinedTypes
                                  .Where(type => type is { IsAbstract: false, IsInterface: false }
                                              && type.IsAssignableTo(typeof(IEndpoint)))
                                  .Select(type => ServiceDescriptor.Scoped(typeof(IEndpoint), type));
        services.TryAddEnumerable(descriptors);

        return services;
    }

    /// <summary>
    /// Maps all the registered endpoints.
    /// </summary>
    /// <param name="app"><see cref="WebApplication"/> instance.</param>
    /// <param name="group"><see cref="RouteGroupBuilder"/> instance.</param>
    /// <returns>Returns <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? group = default)
    {
        IEndpointRouteBuilder builder = group is null ? app : group;

        var endpoints = app.Services
                           .GetRequiredService<IServiceScopeFactory>()
                           .CreateScope()
                           .ServiceProvider
                           .GetRequiredService<IEnumerable<IEndpoint>>();
        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}

namespace OpenChat.PlaygroundApp.Endpoints;

/// <summary>
/// This provides interfaces to the endpoints.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Maps the endpoint to the specified <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="app"><see cref="IEndpointRouteBuilder"/> instance.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}

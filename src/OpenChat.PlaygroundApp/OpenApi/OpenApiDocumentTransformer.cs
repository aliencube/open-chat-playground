using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace OpenChat.PlaygroundApp.OpenApi;

/// <summary>
/// This represents the transformer entity for OpenAPI document.
/// </summary>
public class OpenApiDocumentTransformer(IHttpContextAccessor accessor) : IOpenApiDocumentTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "OpenChat Playground API",
            Version = "1.0.0",
            Description = "An API for the OpenChat Playground."
        };
        document.Servers =
        [
            new OpenApiServer
            {
                Url = accessor.HttpContext != null
                    ? $"{accessor.HttpContext.Request.Scheme}://{accessor.HttpContext.Request.Host}/"
                    : "http://localhost:5280/"
            }
        ];

        return Task.CompletedTask;
    }
}

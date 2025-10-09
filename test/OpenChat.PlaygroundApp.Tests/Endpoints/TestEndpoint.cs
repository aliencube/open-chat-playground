using Microsoft.AspNetCore.Routing;

using OpenChat.PlaygroundApp.Endpoints;

namespace OpenChat.PlaygroundApp.Tests.Endpoints;

public partial class EndpointExtensionsTests
{
    private class TestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
        }
    }
}
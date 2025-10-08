using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

using OpenChat.PlaygroundApp.Endpoints;
using OpenChat.PlaygroundApp.Services;

namespace OpenChat.PlaygroundApp.Tests.Endpoints;

public class ChatResponseEndpointTests
{
    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_IChatService_When_ChatResponseEndpoint_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ChatResponseEndpoint>>();

        // Act
        Action action = () => new ChatResponseEndpoint(default(IChatService)!, logger);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Logger_When_ChatResponseEndpoint_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var service = Substitute.For<IChatService>();

        // Act
        Action action = () => new ChatResponseEndpoint(service, default(ILogger<ChatResponseEndpoint>)!);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Both_Dependencies_When_ChatResponseEndpoint_Instantiated_Then_It_Should_Create()
    {
        // Arrange
        var service = Substitute.For<IChatService>();
        var logger = Substitute.For<ILogger<ChatResponseEndpoint>>();

        // Act
        var result = new ChatResponseEndpoint(service, logger);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("/chat/responses")]
    public void Given_Endpoint_When_MapEndpoint_Invoked_Then_It_Should_Contain(string pattern)
    {
        // Arrange
        var args = Array.Empty<string>();
        var app = WebApplication.CreateBuilder(args).Build();
        var chatService = Substitute.For<IChatService>();
        var logger = Substitute.For<ILogger<ChatResponseEndpoint>>();
        var endpoint = new ChatResponseEndpoint(chatService, logger);

        // Act
        endpoint.MapEndpoint(app);
        var result = app.GetType()
                        .GetProperty("DataSources", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                        .GetValue(app) as ICollection<EndpointDataSource>;

        // Assert
        result.ShouldNotBeNull()
              .First()
              .Endpoints.OfType<RouteEndpoint>()
              .Any(e => e.RoutePattern.RawText?.Equals(pattern, StringComparison.OrdinalIgnoreCase) ?? false)
              .ShouldBeTrue();
    }
}

using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenChat.PlaygroundApp.Endpoints;
using OpenChat.PlaygroundApp.Services;

namespace OpenChat.PlaygroundApp.Tests.Endpoints;

public partial class EndpointExtensionsTests
{
    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Services_When_AddEndpoints_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        IServiceCollection services = null!;
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        Action action = () => EndpointExtensions.AddEndpoints(services, assembly);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Assembly_When_AddEndpoints_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var services = new ServiceCollection();
        Assembly assembly = null!;

        // Act
        Action action = () => EndpointExtensions.AddEndpoints(services, assembly);

        // Assert
        action.ShouldThrow<NullReferenceException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Assembly_When_AddEndpoints_Invoked_Then_It_Should_Register_Endpoints()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(ChatResponseEndpoint).Assembly;

        // Act
        var result = services.AddEndpoints(assembly);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(services);
        services.Any(sd => sd.ServiceType == typeof(IEndpoint)).ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_Assembly_When_AddEndpoints_Invoked_Then_It_Should_Register_As_Scoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(ChatResponseEndpoint).Assembly;

        // Act
        services.AddEndpoints(assembly);
        var descriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(IEndpoint));

        // Assert
        descriptor.ShouldNotBeNull();
        descriptor.Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(typeof(ChatResponseEndpoint))]
    public void Given_Valid_Assembly_When_AddEndpoints_Invoked_Then_It_Should_Register_All_Endpoint_Implementations(Type endpointType)
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(ChatResponseEndpoint).Assembly;

        // Act
        services.AddEndpoints(assembly);
        var endpointDescriptors = services.Where(sd => sd.ServiceType == typeof(IEndpoint)).ToList();

        // Assert
        endpointDescriptors.ShouldNotBeEmpty();
        endpointDescriptors.Any(sd => sd.ImplementationType == endpointType).ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Assembly_With_IEndpoint_Implementations_When_AddEndpoints_Invoked_Then_It_Should_Register_Them()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(EndpointExtensionsTests).Assembly;

        // Act
        services.AddEndpoints(assembly);
        var endpointDescriptors = services.Where(sd => sd.ServiceType == typeof(IEndpoint)).ToList();

        // Assert
        endpointDescriptors.ShouldNotBeEmpty();
        endpointDescriptors.Any(sd => sd.ImplementationType == typeof(TestEndpoint)).ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_App_When_MapEndpoints_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        WebApplication app = null!;

        // Act
        Action action = () => EndpointExtensions.MapEndpoints(app);

        // Assert
        action.ShouldThrow<NullReferenceException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_App_With_TestEndpoint_When_MapEndpoints_Invoked_Then_It_Should_Return_App()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddEndpoints(typeof(EndpointExtensionsTests).Assembly);
        var app = builder.Build();

        // Act
        var result = app.MapEndpoints();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(app);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_App_With_Endpoints_When_MapEndpoints_Invoked_Then_It_Should_Map_All_Endpoints()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var chatService = Substitute.For<IChatService>();
        var logger = Substitute.For<ILogger<ChatResponseEndpoint>>();
        
        builder.Services.AddSingleton(chatService);
        builder.Services.AddSingleton(logger);
        builder.Services.AddEndpoints(typeof(ChatResponseEndpoint).Assembly);
        var app = builder.Build();

        // Act
        var result = app.MapEndpoints();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(app);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Valid_App_And_RouteGroup_When_MapEndpoints_Invoked_Then_It_Should_Map_Endpoints_To_Group()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var chatService = Substitute.For<IChatService>();
        var logger = Substitute.For<ILogger<ChatResponseEndpoint>>();
        
        builder.Services.AddSingleton(chatService);
        builder.Services.AddSingleton(logger);
        builder.Services.AddEndpoints(typeof(ChatResponseEndpoint).Assembly);
        var app = builder.Build();
        var group = app.MapGroup("/api");

        // Act
        var result = app.MapEndpoints(group);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(app);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Multiple_Calls_When_AddEndpoints_Invoked_Then_It_Should_Not_Duplicate_Registrations()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(ChatResponseEndpoint).Assembly;

        // Act
        services.AddEndpoints(assembly);
        var initialCount = services.Count(sd => sd.ServiceType == typeof(IEndpoint));

        services.AddEndpoints(assembly);
        var finalCount = services.Count(sd => sd.ServiceType == typeof(IEndpoint));

        // Assert
        finalCount.ShouldBe(initialCount);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Assembly_Without_Abstract_Or_Interface_Types_When_AddEndpoints_Invoked_Then_It_Should_Only_Register_Concrete_Types()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(ChatResponseEndpoint).Assembly;

        // Act
        services.AddEndpoints(assembly);
        var endpointDescriptors = services.Where(sd => sd.ServiceType == typeof(IEndpoint)).ToList();

        // Assert
        endpointDescriptors.ShouldNotBeEmpty();
        endpointDescriptors.All(sd => sd.ImplementationType is not null && 
                                      !sd.ImplementationType.IsAbstract && 
                                      !sd.ImplementationType.IsInterface).ShouldBeTrue();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_RouteGroupBuilder_When_MapEndpoints_Invoked_Then_It_Should_Use_Group_As_Builder()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddEndpoints(typeof(EndpointExtensionsTests).Assembly);
        var app = builder.Build();
        var group = app.MapGroup("/test");

        // Act
        var result = app.MapEndpoints(group);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(app);
    }
}
namespace OpenChat.PlaygroundApp.Tests;

using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Threading.Tasks;

public class ProgramTest
{
    /// <summary>
    /// Ollama ChatClient DI Unit Test
    /// </summary>
    [Fact]
    public void GivenBuilder_WhenOllamaRegisteredWithOldMethod_ThenItPersisted()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        string deploymentName = "llama"; // In prod, it reads config

        // Act
        builder.AddOllamaSharpChatClient(deploymentName);
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var ollamaApiClient = serviceProvider.GetService(typeof(IOllamaApiClient));
        var chatClient = serviceProvider.GetService(typeof(IChatClient));

        Assert.NotNull(ollamaApiClient);
        Assert.NotNull(chatClient);
    }

    /// <summary>
    /// Ollama ChatClient DI Unit Test
    /// </summary>
    [Fact]
    public void GivenBuilder_WhenOllamaRegistered_ThenItPersisted()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        string deploymentName = "http://localhost:11434"; // In prod, it reads config

        // Act
        builder.AddOllamaApiClient(deploymentName).AddChatClient();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var ollamaApiClient = serviceProvider.GetService(typeof(IOllamaApiClient));
        var chatClient = serviceProvider.GetService(typeof(IChatClient));

        Assert.NotNull(ollamaApiClient);
        Assert.NotNull(chatClient);
    }

    /// <summary>
    /// Ollama ChatClient Ollama model creation test
    /// </summary>
    [Fact]
    public async Task GivenOllamaService_WhenOllamaModelRequested_ThenModelNotNull()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        string deploymentName = "llama"; // In prod, it reads config
        builder.AddOllamaApiClient(deploymentName).AddChatClient();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Act
        var ollamaApiClient = serviceProvider.GetService(typeof(IOllamaApiClient)) as IOllamaApiClient;
        var chatClient = serviceProvider.GetService(typeof(IChatClient)) as IChatClient;

        // Assert
        Assert.NotNull(chatClient);
        Assert.NotNull(ollamaApiClient);
        var response = await chatClient.GetResponseAsync("Why is the sky blue?");
        Console.WriteLine(response.Messages);
    }
}
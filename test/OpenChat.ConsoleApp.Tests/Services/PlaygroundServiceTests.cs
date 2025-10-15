using OpenChat.ConsoleApp.Clients;
using OpenChat.ConsoleApp.Models;
using OpenChat.ConsoleApp.Services;

namespace OpenChat.ConsoleApp.Tests.Services;

public class PlaygroundServiceTests
{
    private readonly IApiClient _client;
    private readonly PlaygroundService _service;

    public PlaygroundServiceTests()
    {
        this._client = Substitute.For<IApiClient>();
        this._service = new PlaygroundService(this._client);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Constructor_WithNullClient_ShouldThrowArgumentNullException()
    {
        // Act
        Action action = () => new PlaygroundService(null!);

        // Act & Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Constructor_WithValidClient_ShouldCreateInstance()
    {
        // Act
        var service = new PlaygroundService(this._client);

        // Assert
        service.ShouldNotBeNull();
        service.ShouldBeOfType<PlaygroundService>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task RunAsync_WithEmptyInput_ShouldExitGracefully()
    {
        // Arrange
        using var reader = new StringReader(string.Empty);
        Console.SetIn(reader);

        using var writer = new StringWriter();
        Console.SetOut(writer);

        // Act
        await _service.RunAsync();
        var output = writer.ToString();

        // Assert
        output.ShouldContain("User: ");
        await this._client.DidNotReceive()
                          .InvokeStreamAsync(Arg.Any<IEnumerable<ChatMessage>>());
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task RunAsync_WithSingleUserMessage_ShouldProcessCorrectly()
    {
        // Arrange
        var input = "Hello, how are you?\n\n"; // Empty line to exit
        using var reader = new StringReader(input);
        Console.SetIn(reader);

        using var writer = new StringWriter();
        Console.SetOut(writer);

        var responses = CreateMockStreamResponse("I'm doing well, thank you!");
        this._client.InvokeStreamAsync(Arg.Any<IEnumerable<ChatMessage>>())
                    .Returns(responses);

        // Act
        await this._service.RunAsync();

        // Assert
        var output = writer.ToString();
        output.ShouldContain("User: ");
        output.ShouldContain("Assistant: ");
        output.ShouldContain("I'm doing well, thank you!");
        await this._client.Received(1)
                          .InvokeStreamAsync(
                              Arg.Is<IEnumerable<ChatMessage>>(messages =>
                                  messages.Count() == 2 && // system + user messages
                                  messages.First().Role == "system" &&
                                  messages.First().Message == "You are a helpful assistant." &&
                                  messages.Last().Role == "user" &&
                                  messages.Last().Message == "Hello, how are you?"));
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task RunAsync_WithMultipleMessages_ShouldMaintainConversationHistory()
    {
        // Arrange
        var input = "Hello\nHow are you?\n\n"; // Two messages then empty line to exit
        using var reader = new StringReader(input);
        Console.SetIn(reader);

        using var writer = new StringWriter();
        Console.SetOut(writer);

        var firstResponse = CreateMockStreamResponse("Hello there!");
        var secondResponse = CreateMockStreamResponse("I'm doing great!");

        this._client.InvokeStreamAsync(Arg.Any<IEnumerable<ChatMessage>>())
                    .Returns(firstResponse, secondResponse);

        // Act
        await this._service.RunAsync();

        // Assert
        var output = writer.ToString();
        output.ShouldContain("Hello there!");
        output.ShouldContain("I'm doing great!");
        await this._client.Received(2)
                          .InvokeStreamAsync(Arg.Any<IEnumerable<ChatMessage>>());
        await this._client.Received(1)
                          .InvokeStreamAsync(
                              Arg.Is<IEnumerable<ChatMessage>>(messages =>
                                  messages.Count() == 4 && // system + user + assistant + user
                                  messages.ElementAt(0).Role == "system" &&
                                  messages.ElementAt(1).Role == "user" &&
                                  messages.ElementAt(1).Message == "Hello" &&
                                  messages.ElementAt(2).Role == "assistant" &&
                                  messages.ElementAt(2).Message == "Hello there!" &&
                                  messages.ElementAt(3).Role == "user" &&
                                  messages.ElementAt(3).Message == "How are you?"));
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task RunAsync_WithWhitespaceInput_ShouldIgnoreAndExit()
    {
        // Arrange
        var input = "   \n\n"; // Whitespace then empty line
        using var reader = new StringReader(input);
        Console.SetIn(reader);

        using var writer = new StringWriter();
        Console.SetOut(writer);

        // Act
        await this._service.RunAsync();

        // Assert
        var output = writer.ToString();
        output.ShouldContain("User: ");
        await this._client.DidNotReceive()
                          .InvokeStreamAsync(Arg.Any<IEnumerable<ChatMessage>>());
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task RunAsync_WhenApiClientThrowsException_ShouldCatchAndDisplayError()
    {
        // Arrange
        var input = "Hello\n\n";
        using var reader = new StringReader(input);
        Console.SetIn(reader);

        using var writer = new StringWriter();
        Console.SetOut(writer);

        var expectedError = "API connection failed";
        this._client.InvokeStreamAsync(Arg.Any<IEnumerable<ChatMessage>>())
                    .Returns<IAsyncEnumerable<ChatMessage>>(x => throw new Exception(expectedError));

        // Act
        await this._service.RunAsync();

        // Assert
        var output = writer.ToString();
        output.ShouldContain(expectedError);
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task RunAsync_WithStreamedResponse_ShouldDisplayIncrementalOutput()
    {
        // Arrange
        var input = "Tell me a story\n\n";
        using var reader = new StringReader(input);
        Console.SetIn(reader);

        using var writer = new StringWriter();
        Console.SetOut(writer);

        var responses = CreateMockStreamResponseMultipleParts(["Once", " upon", " a", " time"]);
        this._client.InvokeStreamAsync(Arg.Any<IEnumerable<ChatMessage>>())
                    .Returns(responses);

        // Act
        await this._service.RunAsync();

        // Assert
        var output = writer.ToString();
        output.ShouldContain("Assistant: ");
        output.ShouldContain("Once upon a time");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task RunAsync_WithNonAssistantRoleInStream_ShouldIgnoreMessage()
    {
        // Arrange
        var input = "Hello\n\n";
        using var reader = new StringReader(input);
        Console.SetIn(reader);

        using var writer = new StringWriter();
        Console.SetOut(writer);

        var responses = CreateMockStreamResponseWithDifferentRoles();
        this._client.InvokeStreamAsync(Arg.Any<IEnumerable<ChatMessage>>())
                    .Returns(responses);

        // Act
        await this._service.RunAsync();

        // Assert
        var output = writer.ToString();
        output.ShouldContain("Assistant: ");
        output.ShouldContain("Hello from assistant");
        output.ShouldNotContain("This should be ignored");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task RunAsync_SystemMessageAddedOnlyOnce_ShouldNotDuplicateSystemMessage()
    {
        // Arrange
        var input = "First message\nSecond message\n\n";
        using var reader = new StringReader(input);
        Console.SetIn(reader);

        using var writer = new StringWriter();
        Console.SetOut(writer);

        var firstResponse = CreateMockStreamResponse("First response");
        var secondResponse = CreateMockStreamResponse("Second response");

        this._client.InvokeStreamAsync(Arg.Any<IEnumerable<ChatMessage>>())
                    .Returns(firstResponse, secondResponse);

        // Act
        await this._service.RunAsync();

        // Assert
        await this._client.Received(1)
                          .InvokeStreamAsync(
                              Arg.Is<IEnumerable<ChatMessage>>(messages =>
                                  messages.Count() == 4 && // Should still have only one system message
                                  messages.Count(m => m.Role == "system") == 1));
    }

    private static async IAsyncEnumerable<ChatMessage> CreateMockStreamResponse(string message)
    {
        await Task.Yield(); // Ensure it's truly async

        yield return new ChatMessage { Role = "assistant", Message = message };
    }

    private static async IAsyncEnumerable<ChatMessage> CreateMockStreamResponseMultipleParts(string[] parts)
    {
        foreach (var part in parts)
        {
            await Task.Yield(); // Ensure it's truly async

            yield return new ChatMessage { Role = "assistant", Message = part };
        }
    }

    private static async IAsyncEnumerable<ChatMessage> CreateMockStreamResponseWithDifferentRoles()
    {
        await Task.Yield(); // Ensure it's truly async

        yield return new ChatMessage { Role = "assistant", Message = "Hello from assistant" };
        yield return new ChatMessage { Role = "user", Message = "This should be ignored" };
        yield return new ChatMessage { Role = "system", Message = "This should also be ignored" };
    }
}

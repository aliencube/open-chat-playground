using System.Net;
using System.Text;
using System.Text.Json;

using OpenChat.ConsoleApp.Clients;
using OpenChat.ConsoleApp.Models;

namespace OpenChat.ConsoleApp.Tests.Clients;

/// <summary>
/// Unit tests for the <see cref="ApiClient"/> class.
/// </summary>
public class ApiClientTests : IDisposable
{
    private readonly HttpClient _http;
    private readonly TestHttpMessageHandler _handler;
    private static readonly JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public ApiClientTests()
    {
        this._handler = new TestHttpMessageHandler();
        this._http = new HttpClient(this._handler)
        {
            BaseAddress = new Uri("https://test.example.com/")
        };
    }

    public void Dispose()
    {
        this._http?.Dispose();
        this._handler?.Dispose();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenHttpClientIsNull()
    {
        // Assert
        Action action = () => new ApiClient(null!);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task InvokeStreamAsync_ShouldSendPostRequestToCorrectEndpoint()
    {
        // Arrange
        var messages = new[]
        {
            new ChatMessage { Role = "user", Message = "Hello" }
        };

        var responses = new[]
        {
            new ChatMessage { Role = "assistant", Message = "Hi there!" }
        };
        this._handler.SetupResponse(HttpStatusCode.OK, responses);
        var client = new ApiClient(this._http);

        // Act
        await client.InvokeStreamAsync(messages);

        // Assert
        this._handler.LastRequest.ShouldNotBeNull();
        this._handler.LastRequest.Method.ShouldBe(HttpMethod.Post);
        this._handler.LastRequest.RequestUri?.ToString().ShouldEndWith("api/chat/responses");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task InvokeStreamAsync_ShouldSendCorrectRequestBody()
    {
        // Arrange
        var messages = new[]
        {
            new ChatMessage { Role = "user", Message = "Hello" },
            new ChatMessage { Role = "assistant", Message = "Hi!" }
        };

        var responses = new[]
        {
            new ChatMessage { Role = "assistant", Message = "How can I help?" }
        };

        this._handler.SetupResponse(HttpStatusCode.OK, responses);
        var client = new ApiClient(this._http);

        // Act
        await client.InvokeStreamAsync(messages);

        // Assert
        this._handler.LastRequestContent.ShouldNotBeNull();
        
        var sent = JsonSerializer.Deserialize<ChatMessage[]>(this._handler.LastRequestContent, options);
        sent.ShouldNotBeNull();
        sent.Length.ShouldBe(2);
        sent[0].Role.ShouldBe("user");
        sent[0].Message.ShouldBe("Hello");
        sent[1].Role.ShouldBe("assistant");
        sent[1].Message.ShouldBe("Hi!");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task InvokeStreamAsync_ShouldReturnAsyncEnumerableOfChatMessages()
    {
        // Arrange
        var inputs = new[]
        {
            new ChatMessage { Role = "user", Message = "Hello" }
        };

        var responses = new[]
        {
            new ChatMessage { Role = "assistant", Message = "Hi there!" },
            new ChatMessage { Role = "assistant", Message = "How can I help you today?" }
        };

        this._handler.SetupResponse(HttpStatusCode.OK, responses);
        var client = new ApiClient(this._http);

        // Act
        var result = await client.InvokeStreamAsync(inputs);

        // Assert
        result.ShouldNotBeNull();
        
        var messages = new List<ChatMessage>();
        await foreach (var message in result)
        {
            messages.Add(message);
        }

        messages.Count.ShouldBe(2);
        messages[0].Role.ShouldBe("assistant");
        messages[0].Message.ShouldBe("Hi there!");
        messages[1].Role.ShouldBe("assistant");
        messages[1].Message.ShouldBe("How can I help you today?");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task InvokeStreamAsync_ShouldHandleEmptyResponseStream()
    {
        // Arrange
        var inputs = new[]
        {
            new ChatMessage { Role = "user", Message = "Hello" }
        };

        this._handler.SetupResponse(HttpStatusCode.OK, []);
        var client = new ApiClient(this._http);

        // Act
        var result = await client.InvokeStreamAsync(inputs);

        // Assert
        result.ShouldNotBeNull();
        
        var messages = new List<ChatMessage>();
        await foreach (var message in result)
        {
            messages.Add(message);
        }

        messages.ShouldBeEmpty();
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public void InvokeStreamAsync_ShouldThrowHttpRequestException_WhenResponseIsNotSuccessful(HttpStatusCode statusCode)
    {
        // Arrange
        var messages = new[]
        {
            new ChatMessage { Role = "user", Message = "Hello" }
        };

        this._handler.SetupErrorResponse(statusCode);
        var client = new ApiClient(this._http);

        // Act
        Func<Task> func = async () => await client.InvokeStreamAsync(messages);
        
        // Assert
        func.ShouldThrow<HttpRequestException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task InvokeStreamAsync_ShouldHandleNullMessages()
    {
        // Arrange
        var responses = new[]
        {
            new ChatMessage { Role = "assistant", Message = "Response" }
        };

        this._handler.SetupResponse(HttpStatusCode.OK, responses);
        var client = new ApiClient(this._http);

        // Act
        var result = await client.InvokeStreamAsync(null!);

        // Assert
        result.ShouldNotBeNull();
        this._handler.LastRequestContent.ShouldBe("null");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public async Task InvokeStreamAsync_ShouldHandleEmptyMessagesList()
    {
        // Arrange
        var responseMessages = new[]
        {
            new ChatMessage { Role = "assistant", Message = "Response" }
        };

        this._handler.SetupResponse(HttpStatusCode.OK, responseMessages);
        var client = new ApiClient(this._http);

        // Act
        var result = await client.InvokeStreamAsync(Array.Empty<ChatMessage>());

        // Assert
        result.ShouldNotBeNull();
        this._handler.LastRequestContent.ShouldBe("[]");
    }

    /// <summary>
    /// Test HTTP message handler for mocking HTTP responses.
    /// </summary>
    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage? _response;
        
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastRequestContent { get; private set; }

        public void SetupResponse(HttpStatusCode statusCode, ChatMessage[] messages)
        {
            var jsonContent = JsonSerializer.Serialize(messages);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            _response = new HttpResponseMessage(statusCode)
            {
                Content = content
            };
        }

        public void SetupErrorResponse(HttpStatusCode statusCode)
        {
            _response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent("Error", Encoding.UTF8, "text/plain")
            };
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            
            if (request.Content is not null)
            {
                LastRequestContent = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            return _response ?? new HttpResponseMessage(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _response?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using OpenChat.ConsoleApp.Clients;
using OpenChat.ConsoleApp.Models;

namespace OpenChat.ConsoleApp.Services;

/// <summary>
/// This provides interfaces to the <see cref="PlaygroundService"/>.
/// </summary>
public interface IPlaygroundService
{
    /// <summary>
    /// Runs the playground service.
    /// </summary>
    Task RunAsync();
}

/// <summary>
/// This represents the service entity for the Playground operation.
/// </summary>
/// <param name="client">The <see cref="IApiClient"/> instance.</param>
public class PlaygroundService(IApiClient client) : IPlaygroundService
{
    private const string SYSTEM_ROLE = "system";
    private const string USER_ROLE = "user";
    private const string ASSISTANT_ROLE = "assistant";

    private readonly IApiClient _client = client ?? throw new ArgumentNullException(nameof(client));

    private readonly List<ChatMessage> _messages = [];

    /// <inheritdoc/>
    public async Task RunAsync()
    {
        try
        {
            while (true)
            {
                Console.Write("User: ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    break;
                }

                var system = new ChatMessage()
                {
                    Role = SYSTEM_ROLE,
                    Message = "You are a helpful assistant."
                };

                if (this._messages.Count == 0)
                {
                    this._messages.Add(system);
                }

                var user = new ChatMessage()
                {
                    Role = USER_ROLE,
                    Message = input
                };
                this._messages.Add(user);

                Console.Write("Assistant: ");

                var assistant = new ChatMessage()
                {
                    Role = ASSISTANT_ROLE,
                    Message = string.Empty
                };
                this._messages.Add(assistant);

                var prompt = this._messages.Take(this._messages.Count - 1);
                var result = await this._client.InvokeStreamAsync([.. prompt]);
                await foreach (var item in result)
                {
                    await Task.Delay(20);

                    if (item is { Role: ASSISTANT_ROLE })
                    {
                        Console.Write(item!.Message);
                        assistant.Message += item.Message;
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

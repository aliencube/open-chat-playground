namespace OpenChat.ConsoleApp.Models;

/// <summary>
/// This represents the chat message entity.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Gets or sets the role of the message.
    /// </summary>
    public required string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    public required string Message { get; set; } = string.Empty;
}

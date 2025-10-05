using System.ComponentModel.DataAnnotations;

namespace OpenChat.PlaygroundApp.Models;

/// <summary>
/// This represents the chat request entity.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// Gets or sets the role of the message sender.
    /// </summary>
    [Required]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    [Required]
    public string Message { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace OpenChat.PlaygroundApp.Models;

/// <summary>
/// This represents the chat response entity.
/// </summary>
public class ChatResponse
{
    /// <summary>
    /// Gets or sets the role of the message content.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    [Required]
    public string Message { get; set; } = string.Empty;
}

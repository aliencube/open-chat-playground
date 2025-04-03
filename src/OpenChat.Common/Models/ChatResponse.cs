namespace OpenChat.Common.Models;

public class ChatResponse(string? message)
{
    public string? Message { get; set; } = message;
}
namespace OpenChat.Common.Models;

public class ChatResponse(string? content)
{
    public string? Content { get; set; } = content;
}
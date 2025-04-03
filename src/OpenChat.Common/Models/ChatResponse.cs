namespace OpenChat.Common.Models;

public class ChatResponse()
{
    public RoleType Role { get; set; }
    public string? AgentName { get; set; }
    public string? Content { get; set; }
}
namespace ChatVerse.Shared.Models;

public class ChatRoom
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public bool IsPrivate { get; set; }
    public string? Topic { get; set; }
    public int? MaxUsers { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

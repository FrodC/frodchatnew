namespace ChatVerse.Shared.Models;

public class Message
{
    public Guid Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentAtUtc { get; set; }
    public Guid ChatRoomId { get; set; }
}

namespace Persistence.Models;

public class ChatMessage
{
    public int MessageId { get; set; }
    public long UserId { get; set; }

    public int GetterMessageId { get; set; }
    public long GetterUserId { get; set; }

    public DateTime SendDate { get; set; }

    public User User { get; set; }
    public User GetterUser { get; set; }
}

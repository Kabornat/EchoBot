namespace Persistence.Models;

public class User
{
    public long Id { get; set; }
    public bool GetMessages { get; set; }
    public DateTime LastMessageSend { get; set; }
    public Status Status { get; set; }
}

public enum Status
{
    None,
    Ok,
    Muted,
    Banned,
    Admin
}
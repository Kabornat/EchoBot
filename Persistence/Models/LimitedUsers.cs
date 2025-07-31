namespace Persistence.Models;

public class LimitedUser
{
    public long UserId { get; set; }
    public DateTime LimitPeriod { get; set; }
}

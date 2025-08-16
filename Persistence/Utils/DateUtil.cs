namespace Persistence.Utils;

public static class DateUtil
{
    public static TimeSpan GetUntilNext(int days)
    {
        var utcNow = DateTime.UtcNow;

        var nextMidnight = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day).AddDays(days);

        return nextMidnight - utcNow;
    }
}

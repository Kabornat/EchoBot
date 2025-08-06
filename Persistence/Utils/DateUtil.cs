namespace Persistence.Utils;

public static class DateUtil
{
    public static TimeSpan GetUntilNextDay()
    {
        var utcNow = DateTime.UtcNow;

        var nextMidnight = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day).AddDays(1);

        return nextMidnight - utcNow;
    }
}

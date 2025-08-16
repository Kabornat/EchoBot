using Persistence.Utils;

namespace EchoBot.BackgroundServices;

public class TimerManagerService(
    TimerManagerDelayWeek timerManagerDelayWeek)
{
    private readonly Timer _timerDelayWeek = new(timerManagerDelayWeek.Execute, null, TimeSpan.Zero, DateUtil.GetUntilNext(7));
}

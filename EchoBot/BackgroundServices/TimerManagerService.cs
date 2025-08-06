using Persistence.OtherServices;
using Persistence.Utils;

namespace EchoBot.BackgroundServices;

public class TimerManagerService(
    DumpService dumpService,
    TimerManagerDelayWeek timerManagerDelayWeek)
{
    private readonly Timer _timerDelayDay = new(dumpService.CreateNewAndKeep30, null, TimeSpan.Zero, DateUtil.GetUntilNextDay());
    //private readonly Timer _timerDelayWeek = new(timerManagerDelayWeek.Execute, null, TimeSpan.Zero, DateUtil.GetUntilNextDay());
}

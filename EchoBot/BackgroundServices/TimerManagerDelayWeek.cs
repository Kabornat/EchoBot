using Persistence.Services;

namespace EchoBot.BackgroundServices;

public class TimerManagerDelayWeek(
    UserService userService,
    LimitedUserService limitedUserService)
{
    private readonly UserService _userService = userService;
    private readonly LimitedUserService _limitedUserService = limitedUserService;

    public async void Execute(object? state)
    {
        //await _userService.LeaveDelayWeekAsync();
        await _limitedUserService.UnmutePeriodReachedAsync();
    }
}

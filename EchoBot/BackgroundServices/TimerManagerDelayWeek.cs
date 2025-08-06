using Persistence.Services;

namespace EchoBot.BackgroundServices;

public class TimerManagerDelayWeek(UserService userService)
{
    private readonly UserService _userService = userService;

    public async void Execute(object? state)
    {
        await _userService.LeaveDelayWeekAsync();
    }
}

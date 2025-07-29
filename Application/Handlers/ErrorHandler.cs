using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Application.Handlers;

public class ErrorHandler
{
    public async Task HandleAsync(ITelegramBotClient botClient, Exception ex, HandleErrorSource errorSource, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            switch (errorSource)
            {
                case HandleErrorSource.PollingError:
                    Console.WriteLine($"[{nameof(HandleErrorSource.PollingError)}] {ex}");
                    break;

                case HandleErrorSource.FatalError:
                    Console.WriteLine($"[{nameof(HandleErrorSource.FatalError)}] {ex}");
                    break;

                case HandleErrorSource.HandleUpdateError:
                    Console.WriteLine($"[{nameof(HandleErrorSource.HandleUpdateError)}] {ex}");
                    break;

                default:
                    Console.WriteLine($"[DEFAULT] {ex}");
                    break;
            }
        }, cancellationToken);
    }
}

using Application.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Application;

public class Startup(
    TelegramBotClient botClient,
    CancellationTokenSource cancellationTokenSource,
    UpdateHandler updateHandler,
    ErrorHandler errorHandler,
    CommandsList commandsList)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly CancellationTokenSource _cancellationTokenSource = cancellationTokenSource;
    private readonly UpdateHandler _updateHandler = updateHandler;
    private readonly ErrorHandler _errorHandler = errorHandler;
    private readonly CommandsList _commandsList = commandsList;

    public async Task RunAsync()
    {
        Console.WriteLine("bot starting..");

        var options = new ReceiverOptions()
        {
            DropPendingUpdates = true,
        };

        await _commandsList.OnCommandsAsync();

        _botClient.StartReceiving(
            receiverOptions: options,
            cancellationToken: _cancellationTokenSource.Token,
            updateHandler: _updateHandler.HandleAsync,
            errorHandler: _errorHandler.HandleAsync);

        Console.WriteLine("bot started!");

        while (!_cancellationTokenSource.IsCancellationRequested)
            await Task.Delay(1000);

        Console.WriteLine("bot stoped!");
    }
}

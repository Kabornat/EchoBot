using Telegram.Bot;
using Telegram.Bot.Types;
using Application.Commands;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;

public class AdminMessageTextHandler(
    TelegramBotClient botClient,
    CancellationTokenSource cancellationTokenSource,
    BotCommands botCommands)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly CancellationTokenSource _cancellationTokenSource = cancellationTokenSource;
    private readonly BotCommands _botCommands = botCommands;

    public async Task HandleAsync(Message message)
    {
        var messageText = message.Text;

        if (_botCommands.Stop(messageText))
            await StopHandle(message);

        else if (_botCommands.Sql(messageText))
            await SqlHandle(message);
    }

    private async Task StopHandle(Message message)
    {
        var chatId = message.Chat.Id;

        await _botClient.SendMessage(chatId, "бот был остановлен");

        await _cancellationTokenSource.CancelAsync();
    }

    private async Task SqlHandle(Message message)
    {
        var chatId = message.Chat.Id;

        var query = message.Text.Replace(_botCommands.SqlCommand, "");

        await _botClient.SendMessage(chatId, query);
    }
}

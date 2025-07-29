using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;

public class AdminMessageTextHandler(
    TelegramBotClient botClient,
    CancellationTokenSource cancellationTokenSource)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly CancellationTokenSource _cancellationTokenSource = cancellationTokenSource;

    public async Task HandleAsync(Message message)
    {
        var messageText = message.Text;

        var chatId = message.Chat.Id;

        if (messageText == "/stopBotEcho")
        {
            await _botClient.SendMessage(chatId, "бот был остановлен");
            await _cancellationTokenSource.CancelAsync();
        }
    }
}

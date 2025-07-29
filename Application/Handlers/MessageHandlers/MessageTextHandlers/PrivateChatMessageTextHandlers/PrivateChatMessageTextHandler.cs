using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.PrivateChatMessageTextHandlers;

public class PrivateChatMessageTextHandler(TelegramBotClient botClient)
{
    private readonly TelegramBotClient _botClient = botClient;

    public async Task HandleAsync(Message message)
    {
        var messageText = message.Text;

        var chatId = message.Chat.Id;

        await _botClient.SendMessage(chatId, messageText);
    }
}

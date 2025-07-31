using Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.PrivateChatMessageTextHandlers;
using Telegram.Bot.Types;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers;

public class MessageTextHandler(
    AdminMessageTextHandler adminMessageTextHandler,
    PrivateChatMessageTextHandler privateChatMessageTextHandler)
{
    private readonly AdminMessageTextHandler _adminMessageTextHandler = adminMessageTextHandler;
    private readonly PrivateChatMessageTextHandler _privateChatMessageTextHandler = privateChatMessageTextHandler;

    public async Task HandleAsync(Message message)
    {
        var chatId = message.Chat.Id;

        var userId = message.From.Id;

        if (userId == 1778638961)
        {
            await _adminMessageTextHandler.HandleAsync(message);
        }
        if (chatId == userId)
        {
            await _privateChatMessageTextHandler.HandleAsync(message);
        }
    }
}

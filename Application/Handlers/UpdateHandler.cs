using Application.Handlers.MessageHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers;

public class UpdateHandler(MessageHandler messageHandler)
{
    private readonly MessageHandler _messageHandler = messageHandler;

    public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                await _messageHandler.HandleAsync(update.Message);
                break;
        }
    }
}

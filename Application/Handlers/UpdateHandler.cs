using Application.Handlers.MessageHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers;

public class UpdateHandler(
    MessageHandler messageHandler,
    ErrorHandler errorHandler)
{
    private readonly MessageHandler _messageHandler = messageHandler;
    private readonly ErrorHandler _errorHandler = errorHandler;

    public async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await _messageHandler.HandleAsync(update.Message);
                        break;
                }
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleAsync(botClient, ex, HandleErrorSource.HandleUpdateError, cancellationToken);
            }
        }, cancellationToken);
    }
}

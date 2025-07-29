using Application.Handlers.MessageHandlers.MessageTextHandlers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers.MessageHandlers;

public class MessageHandler(MessageTextHandler messageTextHandler)
{
    private readonly MessageTextHandler _messageTextHandler = messageTextHandler;

    public async Task HandleAsync(Message message)
    {
        switch (message.Type)
        {
            case MessageType.Text:
                await _messageTextHandler.HandleAsync(message);
                break;
        }
    }
}

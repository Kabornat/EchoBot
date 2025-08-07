using Application.Handlers.MessageHandlers.MessageTextHandlers;
using Application.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers.MessageHandlers;

public class MessageHandler(
    MessageTextHandler messageTextHandler,
    SendMessageService sendMessageService)
{
    private readonly MessageTextHandler _messageTextHandler = messageTextHandler;
    private readonly SendMessageService _sendMessageService = sendMessageService;

    public async Task HandleAsync(Message message)
    {
        if (message.From.Id != message.Chat.Id)
            return;

        switch (message.Type)
        {
            case MessageType.Text:
                await _messageTextHandler.HandleAsync(message);
                break;

            default:
                await _sendMessageService.SendAsync(message);
                break;
        }
    }
}

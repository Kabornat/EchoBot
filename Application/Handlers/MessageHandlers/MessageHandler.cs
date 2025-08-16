using Application.Handlers.MessageHandlers.MessageTextHandlers;
using Application.Services;
using Persistence.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers.MessageHandlers;

public class MessageHandler(
    MessageTextHandler messageTextHandler,
    EchoChatService echoChatServiceService,
    UserService userService)
{
    private readonly MessageTextHandler _messageTextHandler = messageTextHandler;
    private readonly EchoChatService _echoChatServiceService = echoChatServiceService;
    private readonly UserService _userService = userService;

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
                await DefaultHandle(message);
                break;
        }
    }

    public async Task DefaultHandle(Message message)
    {
        var userId = message.From.Id;

        var user = await _userService.GetAndRegIfNoneAsync(userId);

        await _echoChatServiceService.SendMessageAsync(message, user);
    }
}

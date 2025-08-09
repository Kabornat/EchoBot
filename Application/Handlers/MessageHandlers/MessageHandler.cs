using Application.Handlers.MessageHandlers.MessageTextHandlers;
using Application.Services;
using Persistence.Models;
using Persistence.OtherModels;
using Persistence.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers.MessageHandlers;

public class MessageHandler(
    MessageTextHandler messageTextHandler,
    EchoChatService sendMessageService,
    BotData botData,
    UserService userService)
{
    private readonly MessageTextHandler _messageTextHandler = messageTextHandler;
    private readonly EchoChatService _sendMessageService = sendMessageService;
    private readonly BotData _botData = botData;
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

        var status = user.Status;


        var rank = status switch
        {
            Status.Admin => Rank.Admin,
            _ => Rank.User,
        };

        if (userId == _botData.OwnerUserId)
            rank = Rank.Owner;

        await _sendMessageService.SendMessageAsync(message, user, rank);
    }
}

using Application.Services;
using Telegram.Bot.Types;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;

public class UserMessageTextHandler(
    SendMessageService sendMessageService,
    UserTextCommandsHandler baseCommandsHandler)
{
    private readonly SendMessageService _sendMessageService = sendMessageService;
    private readonly UserTextCommandsHandler _baseCommandsHandler = baseCommandsHandler;

    public async Task HandleAsync(Message message)
    {
        if (await _baseCommandsHandler.HandleAsync(message, Rank.User))
            return;

        else
            await _sendMessageService.SendAsync(message);
    }
}

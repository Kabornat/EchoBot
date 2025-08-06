using Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;
using Application.Services;
using Telegram.Bot.Types;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;

public class AdminMessageTextHandler(
    UserTextCommandsHandler baseCommandsHandler,
    AdminTextCommandsHandler adminCommandsHandler,
    SendMessageService sendMessageService)
{
    private readonly UserTextCommandsHandler _baseCommandsHandler = baseCommandsHandler;
    private readonly AdminTextCommandsHandler _adminCommandsHandler = adminCommandsHandler;
    private readonly SendMessageService _sendMessageService = sendMessageService;

    public async Task HandleAsync(Message message)
    {
        if (await _baseCommandsHandler.HandleAsync(message, Rank.Admin))
            return;

        if (await _adminCommandsHandler.HandleAsync(message))
            return;

        await _sendMessageService.SendAsync(message);
    }
}

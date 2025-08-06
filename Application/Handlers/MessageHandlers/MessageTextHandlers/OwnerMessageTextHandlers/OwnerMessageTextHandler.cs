using Application.Commands;
using Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;
using Application.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.OwnerMessageTextHandlers;

public class OwnerMessageTextHandler(
    OwnerTextCommandsHandler ownerTextCommandsHandler,
    AdminTextCommandsHandler adminCommandsHandler,
    UserTextCommandsHandler baseCommandsHandler,
    SendMessageService sendMessageService)
{
    private readonly OwnerTextCommandsHandler _ownerTextCommandsHandler = ownerTextCommandsHandler;
    private readonly AdminTextCommandsHandler _adminCommandsHandler = adminCommandsHandler;
    private readonly UserTextCommandsHandler _baseCommandsHandler = baseCommandsHandler;
    private readonly SendMessageService _sendMessageService = sendMessageService;

    public async Task HandleAsync(Message message)
    {
        if (await _baseCommandsHandler.HandleAsync(message, Rank.Owner))
            return;

        if (await _adminCommandsHandler.HandleAsync(message))
            return;

        if (await _ownerTextCommandsHandler.HandleAsync(message))
            return;

        await _sendMessageService.SendAsync(message);
    }
}

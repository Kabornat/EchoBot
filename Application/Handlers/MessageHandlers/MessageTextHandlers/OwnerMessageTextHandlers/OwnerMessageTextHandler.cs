using Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;
using Application.Services;
using Telegram.Bot.Types;
using User = Persistence.Models.User;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.OwnerMessageTextHandlers;

public class OwnerMessageTextHandler(
    OwnerTextCommandsHandler ownerTextCommandsHandler,
    AdminTextCommandsHandler adminCommandsHandler,
    UserTextCommandsHandler baseCommandsHandler,
    EchoChatService sendMessageService)
{
    private readonly OwnerTextCommandsHandler _ownerTextCommandsHandler = ownerTextCommandsHandler;
    private readonly AdminTextCommandsHandler _adminCommandsHandler = adminCommandsHandler;
    private readonly UserTextCommandsHandler _baseCommandsHandler = baseCommandsHandler;
    private readonly EchoChatService _sendMessageService = sendMessageService;

    public async Task HandleAsync(Message message, User user)
    {
        if (await _baseCommandsHandler.HandleAsync(message, Rank.Owner))
            return;

        else if (await _adminCommandsHandler.HandleAsync(message))
            return;

        else if(await _ownerTextCommandsHandler.HandleAsync(message))
            return;

        else
            await _sendMessageService.SendMessageAsync(message, user, Rank.Owner);
    }
}

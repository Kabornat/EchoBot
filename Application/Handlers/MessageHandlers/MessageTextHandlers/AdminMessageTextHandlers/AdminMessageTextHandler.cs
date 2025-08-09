using Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;
using Application.Services;
using Telegram.Bot.Types;
using User = Persistence.Models.User;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;

public class AdminMessageTextHandler(
    UserTextCommandsHandler baseCommandsHandler,
    AdminTextCommandsHandler adminCommandsHandler,
    EchoChatService sendMessageService)
{
    private readonly UserTextCommandsHandler _baseCommandsHandler = baseCommandsHandler;
    private readonly AdminTextCommandsHandler _adminCommandsHandler = adminCommandsHandler;
    private readonly EchoChatService _sendMessageService = sendMessageService;

    public async Task HandleAsync(Message message, User user)
    {
        if (await _baseCommandsHandler.HandleAsync(message, Rank.Admin))
            return;

        if (await _adminCommandsHandler.HandleAsync(message))
            return;

        await _sendMessageService.SendMessageAsync(message, user, Rank.Admin);
    }
}

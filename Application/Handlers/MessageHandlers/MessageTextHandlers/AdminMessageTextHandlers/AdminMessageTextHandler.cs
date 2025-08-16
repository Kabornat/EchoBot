using Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;
using Application.Services;
using Telegram.Bot.Types;
using User = Persistence.Models.User;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;

public class AdminMessageTextHandler(
    UserTextCommandsHandler userCommandsHandler,
    AdminTextCommandsHandler adminCommandsHandler,
    EchoChatService echoChatServiceService)
{
    private readonly UserTextCommandsHandler _userCommandsHandler = userCommandsHandler;
    private readonly AdminTextCommandsHandler _adminCommandsHandler = adminCommandsHandler;
    private readonly EchoChatService _echoChatServiceService = echoChatServiceService;

    public async Task HandleAsync(Message message, User user)
    {
        if (await _userCommandsHandler.HandleAsync(message, Rank.Admin))
            return;

        if (await _adminCommandsHandler.HandleAsync(message))
            return;

        await _echoChatServiceService.SendMessageAsync(message, user);
    }
}

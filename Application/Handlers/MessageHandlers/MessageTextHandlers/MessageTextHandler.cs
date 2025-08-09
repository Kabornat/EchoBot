using Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.OwnerMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;
using Persistence.Models;
using Persistence.OtherModels;
using Persistence.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers;

public class MessageTextHandler(
    BotData botData,
    OwnerMessageTextHandler ownerMessageTextHandler,
    AdminMessageTextHandler adminMessageTextHandler,
    UserMessageTextHandler userMessageTextHandler,
    TelegramBotClient botClient,
    UserService userService)
{
    private readonly BotData _botData = botData;
    private readonly OwnerMessageTextHandler _ownerMessageTextHandler = ownerMessageTextHandler;
    private readonly AdminMessageTextHandler _adminMessageTextHandler = adminMessageTextHandler;
    private readonly UserMessageTextHandler _userMessageTextHandler = userMessageTextHandler;
    private readonly TelegramBotClient _botClient = botClient;
    private readonly UserService _userService = userService;

    public async Task HandleAsync(Message message)
    {
        var userId = message.From.Id;

        var user = await _userService.GetAndRegIfNoneAsync(userId);

        var status = user.Status;


        if (userId == _botData.OwnerUserId)
        {
            try
            {
                if (userId == _botData.OwnerUserId)
                    await _ownerMessageTextHandler.HandleAsync(message, user);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(_botData.OwnerUserId, ex.Message);
            }
        }

        else if(status == Status.Admin)
            await _adminMessageTextHandler.HandleAsync(message, user);

        else
            await _userMessageTextHandler.HandleAsync(message, user);
    }
}

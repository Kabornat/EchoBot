using Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.OwnerMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;
using Persistence.Models;
using Persistence.OtherModels;
using Persistence.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers;

public class MessageTextHandler(
    BotData botData,
    OwnerMessageTextHandler ownerMessageTextHandler,
    AdminMessageTextHandler adminMessageTextHandler,
    UserMessageTextHandler userMessageTextHandler,
    LimitedUserService limitedUserService,
    TelegramBotClient botClient,
    UserService userService)
{
    private readonly BotData _botData = botData;
    private readonly OwnerMessageTextHandler _ownerMessageTextHandler = ownerMessageTextHandler;
    private readonly AdminMessageTextHandler _adminMessageTextHandler = adminMessageTextHandler;
    private readonly UserMessageTextHandler _userMessageTextHandler = userMessageTextHandler;
    private readonly LimitedUserService _limitedUserService = limitedUserService;
    private readonly TelegramBotClient _botClient = botClient;
    private readonly UserService _userService = userService;

    public async Task HandleAsync(Message message)
    {
        var userId = message.From.Id;

        var status = await _userService.GetStatus(userId);


        if (status == Status.Muted)
            await MutedHandle(message);

        else if (status == Status.Banned)
            await BannedHandle(message);


        else if (userId == _botData.OwnerUserId)
        {
            try
            {
                if (userId == _botData.OwnerUserId)
                    await _ownerMessageTextHandler.HandleAsync(message);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(_botData.OwnerUserId, ex.Message);
            }
        }

        else if(status == Status.Admin)
            await _adminMessageTextHandler.HandleAsync(message);

        else
            await _userMessageTextHandler.HandleAsync(message);
    }

    public async Task MutedHandle(Message message)
    {
        var userId = message.From.Id;

        var period = await _limitedUserService.GetPeriod(userId);

        await _botClient.SendMessage(
            userId,

            "🤐 <b>Вам был выдан мут</b>\n\n" +

            $"Вы снова сможете писать в: {period:dd.MM.yyyy HH:mm:ss}"

            , ParseMode.Html);
    }

    public async Task BannedHandle(Message message)
    {
        var userId = message.From.Id;

        await _botClient.SendMessage(userId, "🚫 <b>Вы в бане</b>", ParseMode.Html);
    }
}

using Application.Services;
using Persistence.Models;
using Persistence.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = Persistence.Models.User;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;

public class UserMessageTextHandler(
    EchoChatService sendMessageService,
    UserTextCommandsHandler baseCommandsHandler,
    LimitedUserService limitedUserService,
    TelegramBotClient botClient)
{
    private readonly EchoChatService _sendMessageService = sendMessageService;
    private readonly UserTextCommandsHandler _baseCommandsHandler = baseCommandsHandler;
    private readonly LimitedUserService _limitedUserService = limitedUserService;
    private readonly TelegramBotClient _botClient = botClient;

    public async Task HandleAsync(Message message, User user)
    {
        if (await _baseCommandsHandler.HandleAsync(message, Rank.User))
            return;

        else if (user.Status == Status.Muted)
            await MutedHandle(message);

        else if (user.Status == Status.Banned)
            return;

        else
            await _sendMessageService.SendMessageAsync(message, user);
    }

    public async Task MutedHandle(Message message)
    {
        var userId = message.From.Id;

        var period = await _limitedUserService.GetPeriod(userId);

        string responce =
            "🤐 <b>Вам был выдан мут</b>\n\n" +

            $"Вы снова сможете писать в: {period:dd.MM.yyyy HH:mm:ss} (UTC)";

        if (await _limitedUserService.UnmuteAsync(userId, period))
        {
            responce = "😦 Действие мута закончилось, вы снова можете писать";
        }

        await _botClient.SendMessage(
            userId, responce, ParseMode.Html);
    }
}

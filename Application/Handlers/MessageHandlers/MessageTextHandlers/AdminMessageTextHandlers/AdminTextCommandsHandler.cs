using Application.Commands;
using Application.Services;
using Persistence.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;

public class AdminTextCommandsHandler(
    TelegramBotClient botClient,
    ChatMessageService chatMessageService,
    LimitedUserService limitedUserService,
    EchoChatService sendMessageService)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly ChatMessageService _chatMessageService = chatMessageService;
    private readonly LimitedUserService _limitedUserService = limitedUserService;
    private readonly EchoChatService _sendMessageService = sendMessageService;

    public async Task<bool> HandleAsync(Message message)
    {
        var messageText = message.Text;

        if (BotCommands.StartsWithMute(messageText))
            await MuteHandle(message);

        else if (BotCommands.StartsWithUnmute(messageText))
            await UnmuteHandle(message);

        else if(BotCommands.StartsWithBan(messageText))
            await BanHandle(message);

        else if (BotCommands.StartsWithUnban(messageText))
            await UnbanHandle(message);

        else if (BotCommands.Delete(messageText))
            await DeleteHandle(message);

        else
            return false;

        return true;
    }

    public async Task MuteHandle(Message message)
    {
        var messageText = message.Text;

        var arg = messageText.Replace(BotCommands.MuteCommand + " ", string.Empty);

        var messageReplyToMessage = message.ReplyToMessage;

        string responce;


        const string succesful =
            "🤐 <b>Пользователю был выдан мут на {0} с.</b>\n" +
            "Мут продлиться до {1} (UTC)\n" +
            "✉️ Сообщение: {2}";

        const string messageForLimitedUser =
            "🤐 <b>Вам был выдан мут на {0} с.</b>\n" +
            "Мут продлиться до {1} (UTC)\n" +
            "✉️ Сообщение: {2}";

        const string unsucceful = "<b>Не удалось выдать мут</b>";

        var args = arg.Split();

        if (messageReplyToMessage is not null &&
            args.Length > 0)
        {
            var messageForLimitUser = arg.Replace(args[0], string.Empty);

            if (int.TryParse(args[0], out int repliedPeriodInSeconds) &&
                !string.IsNullOrEmpty(messageForLimitUser) &&
                await _chatMessageService.GetUserIdFromReply(messageReplyToMessage.MessageId) is long repliedMessageUserId &&
                repliedMessageUserId != 0 &&
                await _limitedUserService.MuteAsync(repliedMessageUserId, DateTime.UtcNow.AddSeconds(repliedPeriodInSeconds)))
            {
                await SendMessageForLimitedUser(
                repliedMessageUserId,
                string.Format(
                    messageForLimitedUser,
                    repliedPeriodInSeconds,
                    DateTime.UtcNow.AddSeconds(repliedPeriodInSeconds),
                    messageForLimitUser));

                responce = string.Format(
                        succesful,
                        repliedPeriodInSeconds,
                        DateTime.UtcNow.AddSeconds(repliedPeriodInSeconds),
                        messageForLimitUser);
            }
            else
            {
                responce = unsucceful;
            }
        }
        else if(args.Length > 2)
        {
            var messageForLimitUser = arg.Replace(args[0] + ' ' + args[1], string.Empty);

            if (long.TryParse(args[0], out long argMessageUserId) && argMessageUserId != 0 &&
                int.TryParse(args[1], out int periodInSeconds) &&
                !string.IsNullOrEmpty(messageForLimitUser) &&
                await _limitedUserService.MuteAsync(argMessageUserId, DateTime.UtcNow.AddSeconds(periodInSeconds)))
            {
                await SendMessageForLimitedUser(
                    argMessageUserId,
                    string.Format(
                        messageForLimitedUser,
                        periodInSeconds,
                        DateTime.UtcNow.AddSeconds(periodInSeconds),
                        messageForLimitUser));

                responce = string.Format(
                        succesful,
                        periodInSeconds,
                        DateTime.UtcNow.AddSeconds(periodInSeconds),
                        messageForLimitUser);
            }
            else
            {
                responce = unsucceful;
            }
        }
        else
        {
            responce = unsucceful;
        }

        await _botClient.SendMessage(message.From.Id, responce, ParseMode.Html);
    }

    public async Task UnmuteHandle(Message message)
    {
        var messageText = message.Text;

        var arg = messageText.Replace(BotCommands.UnmuteCommand + " ", string.Empty);

        var messageReplyToMessage = message.ReplyToMessage;

        string responce;


        const string succesful =
            "😮 <b>Пользователю был снят мут</b>";

        const string messageForLimitedUser =
            "😮 <b>Вам был снят мут</b>";

        const string unsucceful = "<b>Не удалось снять мут</b>";

        if (messageReplyToMessage is not null &&
            await _chatMessageService.GetUserIdFromReply(messageReplyToMessage.MessageId) is long repliedMessageUserId &&
            repliedMessageUserId != 0 &&
            await _limitedUserService.MuteAsync(repliedMessageUserId, DateTime.UtcNow))
        {
            await SendMessageForLimitedUser(
                repliedMessageUserId,
                messageForLimitedUser);

            responce = succesful;
        }
        else if (long.TryParse(arg, out long argMessageUserId) && argMessageUserId != 0 &&
                await _limitedUserService.UnmuteAsync(argMessageUserId, DateTime.UtcNow))
        {
            await SendMessageForLimitedUser(
                    argMessageUserId,
                    messageForLimitedUser);

            responce = succesful;
        }
        else
        {
            responce = unsucceful;
        }

        await _botClient.SendMessage(message.From.Id, responce, ParseMode.Html);
    }

    private async Task BanHandle(Message message)
    {
        var messageText = message.Text;

        var arg = messageText.Replace(BotCommands.MuteCommand + " ", string.Empty);

        var messageReplyToMessage = message.ReplyToMessage;

        string responce;


        const string succesful =
            "🚫 <b>Пользователю был выдан бан</b>\n" +
            "✉️ Сообщение: {0}";

        const string messageForLimitedUser =
            "🚫 <b>Вам был выдан бан</b>\n" +
            "✉️ Сообщение: {0}";

        const string unsucceful = "<b>Не удалось выдать бан</b>";

        var args = arg.Split();

        if (messageReplyToMessage is not null &&
            args.Length > 0)
        {
            var messageForLimitUser = arg.Replace(args[0], string.Empty);

            if (!string.IsNullOrEmpty(messageForLimitUser) &&
                await _chatMessageService.GetUserIdFromReply(messageReplyToMessage.MessageId) is long repliedMessageUserId &&
                repliedMessageUserId != 0 &&
                await _limitedUserService.BanAsync(repliedMessageUserId))
            {
                await SendMessageForLimitedUser(
                    repliedMessageUserId,
                    string.Format(messageForLimitedUser, messageForLimitUser), true);

                responce = string.Format(succesful, messageForLimitUser);
            }
            else
            {
                responce = unsucceful;
            }
        }
        else if (args.Length > 2)
        {
            var messageForLimitUser = arg.Replace(args[0] + ' ' + args[1], string.Empty);

            if (long.TryParse(args[0], out long argMessageUserId) && argMessageUserId != 0 &&
                !string.IsNullOrEmpty(messageForLimitUser) &&
                await _limitedUserService.BanAsync(argMessageUserId))
            {
                await SendMessageForLimitedUser(
                    argMessageUserId,
                    string.Format(messageForLimitedUser, messageForLimitUser), true);

                responce = string.Format(succesful, messageForLimitUser);
            }
            else
            {
                responce = unsucceful;
            }
        }
        else
        {
            responce = unsucceful;
        }

        await _botClient.SendMessage(message.From.Id, responce, ParseMode.Html);
    }

    private async Task UnbanHandle(Message message)
    {
        var messageText = message.Text;

        var arg = messageText.Replace(BotCommands.UnbanCommand + " ", string.Empty);

        var messageReplyToMessage = message.ReplyToMessage;

        string responce;


        const string succesful =
            "🚫 <b>Пользователю был снят бан</b>";

        const string messageForLimitedUser =
            "🚫 <b>Вам был снят бан</b>";

        const string unsucceful = "<b>Не удалось снять бан</b>";

        if (messageReplyToMessage is not null &&
            await _chatMessageService.GetUserIdFromReply(messageReplyToMessage.MessageId) is long repliedMessageUserId &&
            repliedMessageUserId != 0 &&
            await _limitedUserService.UnbanAsync(repliedMessageUserId))
        {
            await SendMessageForLimitedUser(
                   repliedMessageUserId, messageForLimitedUser);

            responce = succesful;
        }
        else if (long.TryParse(arg, out long argMessageUserId) && argMessageUserId != 0 &&
                await _limitedUserService.UnbanAsync(argMessageUserId))
        {
            await SendMessageForLimitedUser(
                    argMessageUserId, messageForLimitedUser);

            responce = succesful;
        }
        else
        {
            responce = unsucceful;
        }

        await _botClient.SendMessage(message.From.Id, responce, ParseMode.Html);
    }

    private async Task DeleteHandle(Message message)
    {
        await _sendMessageService.DeleteMessageAsync(message);
    }

    public async Task SendMessageForLimitedUser(long userId, string messageText, bool pinMessage = false)
    {
        var message = await _botClient.SendMessage(userId, messageText, ParseMode.Html);

        if (pinMessage)
            await _botClient.PinChatMessage(userId, message.MessageId);
    }
}

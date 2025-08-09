using System.Text;
using Application.Commands;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Models;
using Persistence.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.OwnerMessageTextHandlers;

public class OwnerTextCommandsHandler(
    TelegramBotClient botClient,
    Stop stop,
    UserService userService,
    ChatMessageService chatMessageService,
    IDbContextFactory<AppDbContext> factory)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly Stop _stop = stop;
    private readonly UserService _userService = userService;
    private readonly ChatMessageService _chatMessageService = chatMessageService;
    private readonly IDbContextFactory<AppDbContext> _factory = factory;

    public async Task<bool> HandleAsync(Message message)
    {
        var messageText = message.Text;

        if (BotCommands.ChatMembersCount(messageText))
            await ChatMembersCountHandle(message);

        else if (BotCommands.StartsWithRankUp(messageText))
            await RankUpHandle(message);

        else if (BotCommands.StartsWithRankDown(messageText))
            await RankDownHandle(message);

        else if (BotCommands.Adminlist(messageText))
            await AdminlistHandle(message);

        else if (BotCommands.Banlist(messageText))
            await BanlistHandle(message);

        else if (BotCommands.Mutelist(messageText))
            await MutelistHandle(message);

        else if (BotCommands.StartsWithUserInfo(messageText))
            await UserInfoHandle(message);

        else if (BotCommands.Stop(messageText))
            await StopHandle(message);

        else if (BotCommands.Sql(messageText))
            await SqlHandle(message);

        else
            return false;

        return true;
    }

    private async Task ChatMembersCountHandle(Message message)
    {
        var chatMembersCount = await _userService.GetChatMembersCount();

        var responce = $"В данный момент в эхо {chatMembersCount} человек(а)";

        await _botClient.SendMessage(message.From.Id, responce);
    }

    private async Task RankUpHandle(Message message)
    {
        var messageText = message.Text;

        var arg = messageText.Replace(BotCommands.RankUpCommand, string.Empty);

        var messageReplyToMessage = message.ReplyToMessage;

        string responce;


        const string succesful = "<b>Пользователю была выдана админка</b>";

        if (messageReplyToMessage is not null &&
            await _chatMessageService.GetUserIdFromReply(messageReplyToMessage.MessageId) is long repliedMessageUserId &&
            repliedMessageUserId != 0 &&
            await _userService.SetRankAsync(repliedMessageUserId, Status.Admin))
        {
            responce = succesful;
        }
        else if(long.TryParse(arg, out long argMessageUserId) &&
            argMessageUserId != 0 &&
            await _userService.SetRankAsync(argMessageUserId, Status.Admin))
        {
            responce = succesful;
        }
        else
        {
            responce = "<b>Не удалось выдать админку</b>";
        }

        await _botClient.SendMessage(message.From.Id, responce, ParseMode.Html);
    }

    private async Task RankDownHandle(Message message)
    {
        var messageText = message.Text;

        var arg = messageText.Replace(BotCommands.RankDownCommand, string.Empty);

        var messageReplyToMessage = message.ReplyToMessage;

        string responce;


        const string succesful = "<b>Пользователю была снята админка</b>";

        if (messageReplyToMessage is not null &&
            await _chatMessageService.GetUserIdFromReply(messageReplyToMessage.MessageId) is long repliedMessageUserId &&
            repliedMessageUserId != 0 &&
            await _userService.SetRankAsync(repliedMessageUserId, Status.Ok))
        {
            responce = succesful;
        }
        else if (long.TryParse(arg, out long argMessageUserId) &&
            argMessageUserId != 0 &&
            await _userService.SetRankAsync(argMessageUserId, Status.Ok))
        {
            responce = succesful;
        }
        else
        {
            responce = "<b>Не удалось снять админку</b>";
        }

        await _botClient.SendMessage(message.From.Id, responce, ParseMode.Html);
    }

    private async Task AdminlistHandle(Message message)
    {
        var responce = await _userService.GetAdminsResponceAsync();

        await _botClient.SendMessage(message.From.Id, responce, ParseMode.Html);
    }

    private async Task BanlistHandle(Message message)
    {
        var responce = await _userService.GetBanlistResponceAsync();

        await _botClient.SendMessage(message.From.Id, responce, ParseMode.Html);
    }

    private async Task MutelistHandle(Message message)
    {
        var responce = await _userService.GetMutelistResponceAsync();

        await _botClient.SendMessage(message.From.Id, responce, ParseMode.Html);
    }

    private async Task UserInfoHandle(Message message)
    {
        var messageText = message.Text;

        var arg = messageText.Replace(BotCommands.UserInfoCommand, string.Empty);

        var messageReplyToMessage = message.ReplyToMessage;

        string responce;


        const string succesful =
            "🆔: <code>{0}</code>\n" +
            "Status: <b>{1}</b>";

        if (messageReplyToMessage is not null &&
            await _chatMessageService.GetUserIdFromReply(messageReplyToMessage.MessageId) is long repliedMessageUserId &&
            repliedMessageUserId != 0)
        {
            var status = await _userService.GetStatusAsync(repliedMessageUserId);

            responce = string.Format(succesful, repliedMessageUserId, status);
        }
        else if (long.TryParse(arg, out long argMessageUserId) &&
            argMessageUserId != 0)
        {
            var status = await _userService.GetStatusAsync(argMessageUserId);

            responce = string.Format(succesful, argMessageUserId, status);
        }
        else
        {
            responce = "<b>Не удалось вывести информацию о пользователе</b>";
        }

        await _botClient.SendMessage(message.From.Id, responce, ParseMode.Html);
    }

    private async Task StopHandle(Message message)
    {
        var chatId = message.Chat.Id;

        await _botClient.SendMessage(chatId, "бот был остановлен");

        await _stop.StopAsync();
    }

    private async Task SqlHandle(Message message)
    {
        var chatId = message.Chat.Id;

        var query = message.Text.Replace(BotCommands.SqlCommand, "");

        string responce;

        await using (var dbContext = await _factory.CreateDbContextAsync())
        {
            if (query.Contains("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                var connection = dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = query;

                var reader = await command.ExecuteReaderAsync();

                var resultSb = new StringBuilder();
                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                        resultSb.Append(reader.GetValue(i) + " ");
                    resultSb.AppendLine();
                }

                responce = string.IsNullOrEmpty(resultSb.ToString()) ? "пусто!" : resultSb.ToString();
            }
            else
            {
                if (!query.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
                {
                    responce = "нере";
                }
                else
                {
                    var affectedRows = await dbContext.Database.ExecuteSqlRawAsync(query);

                    responce = "affected rows: " + affectedRows;
                }
            }
        };
        
        await _botClient.SendMessage(chatId, responce);
    }
}

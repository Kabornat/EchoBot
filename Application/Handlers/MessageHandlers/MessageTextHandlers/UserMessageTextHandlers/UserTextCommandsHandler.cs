using Application.Commands;
using Application.Services;
using Persistence.OtherModels;
using Persistence.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;

public class UserTextCommandsHandler(
    TelegramBotClient botClient,
    BotData botData,
    MainTextService mainTextService,
    UserService userService)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly BotData _botData = botData;
    private readonly MainTextService _mainTextService = mainTextService;
    private readonly UserService _userService = userService;

    public async Task<bool> HandleAsync(Message message, Rank status)
    {
        var messageText = message.Text;

        if (BotCommands.Start(messageText))
            await StartHandle(message, status);

        else if (BotCommands.Help(messageText))
            await HelpHandle(message);

        else if (BotCommands.Anon(messageText))
            await AnonHandle(message);

        else if (BotCommands.Status(messageText))
            return true;

        else if (BotCommands.Leave(messageText))
            await LeaveHandle(message);

        else if (BotCommands.Rules(messageText))
            await RulesHandle(message);

        else if (BotCommands.Myid(messageText))
            await MyidHandle(message);

        else
            return false;

        return true;
    }

    private async Task StartHandle(Message message, Rank status)
    {
        var userId = message.From.Id;

        var responce = _mainTextService.GetMainMenuText(status);

        await _botClient.SendMessage(userId, responce, ParseMode.Html);
    }

    private async Task HelpHandle(Message message)
    {
        var userId = message.From.Id;

        var responce =
$@"
❓ <b>Поддержка</b>

1. Насколько бот анонимен?
• Айди может посмотреть только создатель, но делать я это вряд-ли буду дабы не портить интерес

2. Куда писать для поддержки?
• В @string_support_bot
• За просьбой снять ограничения кидаем причину почему вас ограничили и ваш айди, посмотреть его можно отправив команду /myid

3. Претендуешь на админку?
• Пиши в @string_support_bot

4. Канал:
• {_botData.ChannelUrl}

5. Ещё проекты:
• @diz_project
";

        await _botClient.SendMessage(userId, responce, ParseMode.Html, linkPreviewOptions: true);
    }

    private async Task AnonHandle(Message message)
    {
        var userId = message.From.Id;

        var anon = await _userService.SetAnonAsync(userId);

        var responce = anon ? "🟢 Анонимность была включена" : "🔴 Анонимность была выключена";

        await _botClient.SendMessage(userId, responce, ParseMode.Html);
    }

    private async Task LeaveHandle(Message message)
    {
        var userId = message.From.Id;

        string responce;

        if (!await _userService.LeaveAsync(userId))
        {
            responce = "Вас нет в чате, отправьте любое сообщение чтобы войти";
        }
        else
        {
            responce =
                "🚪 <b>Вы вышли из чата</b>\n\n" +

                "Сообщения больше не приходят к вам, но есть шанс что придут, т.к при отправке сообщения формируется список участников взятых из бд\n\n" +

                "При отправке любого сообщения, кроме команд, вы автоматически войдете в чат";
        }

        await _botClient.SendMessage(userId, responce, ParseMode.Html);
    }

    private async Task RulesHandle(Message message)
    {
        var userId = message.From.Id;

        await _botClient.SendMessage(userId,
$@"
⚖️ <b>Правила</b>

<b>Запрещено:</b>

1. 🔞 NSFW
2. ♿ Спам
3. ♿ Флуд
4. ☪️ Задевание религии
5. ⚙️ Сообщение вызывающее вылет
6. 🏳️‍🌈 Без основательная выдача ограничений 
7. 🔒 Разграничение ограниченных без согласования с ограничившим
8. 👤 Присылать персональные данные участника без его одобрения  

<blockquote>Смысл чата общение и споры на любые темы, прошу не нарушать правила, ведь они созданы исключительно для защиты чата и интереса участников, спам надоедает, флуд тоже, NSFW приведет к бану в некоторых регионах</blockquote>
",

            ParseMode.Html);
    }

    private async Task MyidHandle(Message message)
    {
        var userId = message.From.Id;

        await _botClient.SendMessage(userId, $"Ваш 🆔: <code>{userId}</code>", ParseMode.Html);
    }
}
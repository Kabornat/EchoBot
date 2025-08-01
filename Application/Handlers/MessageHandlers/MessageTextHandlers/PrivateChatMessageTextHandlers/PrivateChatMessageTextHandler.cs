using Application.Commands;
using Application.Services;
using Persistence.OtherModels;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.PrivateChatMessageTextHandlers;

public class PrivateChatMessageTextHandler(
    TelegramBotClient botClient,
    BotCommands botCommands,
    SendMessageService sendMessageService,
    BotData botData,
    MainTextService mainTextService)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly BotCommands _botCommands = botCommands;
    private readonly SendMessageService _sendMessageService = sendMessageService;
    private readonly BotData _botData = botData;
    private readonly MainTextService _mainTextService = mainTextService;

    public async Task HandleAsync(Message message)
    {
        if (AdminExecuteCommands(message))
            return;


        var messageText = message.Text;

        if (_botCommands.Start(messageText))
            await StartHandle(message);

        else if (_botCommands.Help(messageText))
            await HelpHandle(message);

        else if(_botCommands.Anon(messageText))
            await AnonHandle(message);

        else
            await _sendMessageService.SendAsync(message);
    }

    public bool AdminExecuteCommands(Message message)
    {
        var messageText = message.Text;

        var userId = message.From.Id;


        if (userId != 1778638961)
            return false;


        if (_botCommands.Start(messageText))
            return true;

        if (_botCommands.Stop(messageText))
            return true;

        if (_botCommands.Sql(messageText))
            return true;

        return false;
    }

    private async Task StartHandle(Message message)
    {
        var userId = message.From.Id;

        var responce = _mainTextService.GetMainMenuText(Status.User);

        await _botClient.SendMessage(userId, responce, ParseMode.Html);
    }

    private async Task HelpHandle(Message message)
    {
        var userId = message.From.Id;

        var responce = 
$@"
❓ <b>Поддержка</b>

1. Насколько бот анонимен?
• Айди может посмотреть создатель в случае необходимости ограничения или разграничения шалунов

2. Куда писать для поддержки?
• В @string_support_bot
• За просьбой снять ограничения кидаем причину почему вас ограничили и ваш айди, посмотреть его можно в @myidbot

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

        var responce = Random.Shared.Next(2) == 0 ? "🟢 Анонимность была включена" : "🔴 Анонимность была выключена";

        await _botClient.SendMessage(userId, responce, ParseMode.Html);
    }
}

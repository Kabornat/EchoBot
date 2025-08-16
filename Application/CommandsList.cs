using Application.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application;

public class CommandsList(
    TelegramBotClient botClient)
{
    private readonly TelegramBotClient _botClient = botClient;

    public async Task OnCommandsAsync()
    {
        var commandsList = new BotCommand[] 
        {
            new(BotCommands.StartCommand, "Главное меню"),
            new(BotCommands.HelpCommand, "FAQ"),
            new(BotCommands.AnonCommand, "Переключение анонимности")
        };

        await _botClient.SetMyCommands(commandsList, new BotCommandScopeAllPrivateChats());
    }

    public async Task OffCommandsAsync()
    {
        var commandsList = new BotCommand[] 
        {
            new(BotCommands.StatusCommand, "Тех работы, ожидайте"),
        };

        await _botClient.SetMyCommands(commandsList, new BotCommandScopeAllPrivateChats());
    }
}

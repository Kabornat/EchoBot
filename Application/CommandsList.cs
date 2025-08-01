using Application.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application;

public class CommandsList(
    TelegramBotClient botClient,
    BotCommands botCommands)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly BotCommands _botCommands = botCommands;

    public async Task OnCommandsAsync()
    {
        var commandsList = new BotCommand[] 
        {
            new(_botCommands.StartCommand, "Главное меню"),
            new(_botCommands.AnonCommand, "Переключение анонимности"),
            new(_botCommands.HelpCommand, "FAQ")
        };

        await _botClient.SetMyCommands(commandsList, new BotCommandScopeAllPrivateChats());
    }

    public async Task OffCommandsAsync()
    {
        var commandsList = new BotCommand[] 
        {
            new("/status", "Тех работы"),
        };

        await _botClient.SetMyCommands(commandsList, new BotCommandScopeAllPrivateChats());
    }
}

using Application.Commands;
using Application.Services;
using Telegram.Bot.Types;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.PrivateChatMessageTextHandlers;

public class PrivateChatMessageTextHandler(
    BotCommands botCommands,
    SendMessageService sendMessageService)
{
    private readonly BotCommands _botCommands = botCommands;
    private readonly SendMessageService _sendMessageService = sendMessageService;

    public async Task HandleAsync(Message message)
    {
        if (AdminExecuteCommands(message))
            return;

        await _sendMessageService.SendAsync(message);
    }

    public bool AdminExecuteCommands(Message message)
    {
        var messageText = message.Text;

        var userId = message.From.Id;


        if (userId != 1778638961)
            return false;


        if (_botCommands.Stop(messageText))
            return true;

        if (_botCommands.Sql(messageText))
            return true;

        return false;
    }
}

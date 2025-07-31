using Persistence.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Services;

public class SendMessageService(
    TelegramBotClient botClient,
    UserService userService)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly UserService _userService = userService;

    public async Task SendAsync(Message message)
    {
        var userId = message.From.Id;

        await _userService.UpdateLastMessageSend(userId);

        await _botClient.SendMessage(userId, message.Text);
    }
}

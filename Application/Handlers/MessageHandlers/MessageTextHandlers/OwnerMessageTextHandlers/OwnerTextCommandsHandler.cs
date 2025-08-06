using System.Text;
using Application.Commands;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application.Handlers.MessageHandlers.MessageTextHandlers.OwnerMessageTextHandlers;

public class OwnerTextCommandsHandler(
    TelegramBotClient botClient,
    Stop stop,
    BotCommands botCommands,
    IDbContextFactory<AppDbContext> factory)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly Stop _stop = stop;
    private readonly BotCommands _botCommands = botCommands;
    private readonly IDbContextFactory<AppDbContext> _factory = factory;

    public async Task<bool> HandleAsync(Message message)
    {
        var messageText = message.Text;

        if (_botCommands.Stop(messageText))
            await StopHandle(message);

        else if (_botCommands.Sql(messageText))
            await SqlHandle(message);

        else 
            return false;

        return true;
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

        var query = message.Text.Replace(_botCommands.SqlCommand, "");

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

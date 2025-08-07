using Microsoft.EntityFrameworkCore;
using Persistence.Models;

namespace Persistence.Repositories;

public class ChatMessageRepository(IDbContextFactory<AppDbContext> factory)
{
    private readonly IDbContextFactory<AppDbContext> _factory = factory;

    public async Task AddAsync(List<ChatMessage> chatMessages)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        await dbContext.AddRangeAsync(chatMessages);

        await dbContext.SaveChangesAsync();
    }

    public async Task<int> GetMessageIdForReply(long userId, int repliedMessageId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        if (!await dbContext.ChatMessages
            .AsNoTracking()
            .AnyAsync(x => x.GetterMessageId == repliedMessageId))
            return 0;

        var messageId = await dbContext.ChatMessages
            .AsNoTracking()
            .Where(x => x.GetterMessageId == repliedMessageId)
            .Select(x => x.MessageId)
            .FirstAsync();

        return await dbContext.ChatMessages
            .AsNoTracking()
            .Where(
                x => x.GetterUserId == userId &&
                x.MessageId == messageId)
            .Select(x => x.GetterMessageId)
            .FirstAsync();
    }


    // Delay month => UtcNow.AddMonths(-1)
    public async Task ClearChat(DateTime delay)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        await dbContext
            .ChatMessages
            .Where(x => x.SendDate < delay)
            .ExecuteDeleteAsync();
    }
}

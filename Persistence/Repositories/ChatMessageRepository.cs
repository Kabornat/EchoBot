using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class ChatMessageRepository(IDbContextFactory<AppDbContext> factory)
{
    private readonly IDbContextFactory<AppDbContext> _factory = factory;

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

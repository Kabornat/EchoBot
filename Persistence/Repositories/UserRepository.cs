using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class UserRepository(IDbContextFactory<AppDbContext> factory)
{
    private readonly IDbContextFactory<AppDbContext> _factory = factory;

    public async Task UpdateLastMessageSend(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        await dbContext.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.LastMessageSend, DateTime.UtcNow));
    }
}

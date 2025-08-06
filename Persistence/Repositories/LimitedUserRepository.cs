using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class LimitedUserRepository(IDbContextFactory<AppDbContext> factory)
{
    private readonly IDbContextFactory<AppDbContext> _factory = factory;

    public async Task<DateTime> GetPeriod(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.LimitedUsers
            .Where(x => x.UserId == userId)
            .Select(x => x.LimitPeriod)
            .FirstAsync();
    }
}

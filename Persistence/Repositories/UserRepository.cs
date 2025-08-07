using Microsoft.EntityFrameworkCore;
using Persistence.Models;

namespace Persistence.Repositories;

public class UserRepository(IDbContextFactory<AppDbContext> factory)
{
    private readonly IDbContextFactory<AppDbContext> _factory = factory;

    public async Task<List<long>> GetMessageGettersAsync()
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .AsNoTracking()
            .Where(x => x.GetMessages && x.Status != Status.Banned)
            .Select(x => x.Id)
            .ToListAsync();
    }
    public async Task UpdateLastMessageSendAndGetMessages(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        await dbContext.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.LastMessageSend, DateTime.UtcNow)
            .SetProperty(p => p.GetMessages, true));
    }

    public async Task<bool> LeaveAsync(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .Where(x => x.Id == userId && x.GetMessages)
            .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.GetMessages, false)) > 0;
    }

    public async Task<bool> LeaveAsync(List<long> users)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .Where(x => users.Contains(x.Id))
            .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.GetMessages, false)) > 0;
    }

    public async Task<bool> LeaveDelayWeekAsync()
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .Where(x => x.GetMessages && x.LastMessageSend < DateTime.Now.AddDays(-7))
            .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.GetMessages, false)) > 0;
    }

    public async Task<Status> GetStatus(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        var status = await dbContext.Users
            .Where(x => x.Id == userId)
            .Select(s => s.Status)
            .FirstOrDefaultAsync();

        if (status is Status.None)
        {
            var user = new User()
            {
                Id = userId,
                GetMessages = true,
                Status = Status.Ok,
            };

            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Status.Ok;
        }

        return status;
    }
}

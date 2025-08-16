using Microsoft.EntityFrameworkCore;
using Persistence.Models;

namespace Persistence.Repositories;

public class UserRepository(IDbContextFactory<AppDbContext> factory)
{
    private readonly IDbContextFactory<AppDbContext> _factory = factory;

    public async Task<List<long>> GetAsync()
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .AsNoTracking()
            .Select(x => x.Id)
            .ToListAsync();
    }

    public async Task<List<long>> GetListAsync(Status status)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Status == status)
            .Select(x => x.Id)
            .ToListAsync();
    }

    public async Task<List<LimitedUser>> GetMutelistAsync()
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.LimitedUsers
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetChatMembersCount()
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .AsNoTracking()
            .CountAsync(x => x.GetMessages && x.Status != Status.Banned);
    }

    public async Task<List<long>> GetMessageGettersAsync()
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .AsNoTracking()
            .Where(x => x.GetMessages && x.Status != Status.Banned)
            .Select(x => x.Id)
            .ToListAsync();
    }

    public async Task<bool> SetAnonAsync(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        await dbContext.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.Anon, p => !p.Anon));

        return await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.Anon)
            .FirstAsync();
    }

    public async Task<bool> SetRankAsync(long userId, Status status)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .Where(x => x.Id == userId && x.Status != status)
            .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.Status, status)) > 0;
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

    public async Task<Status> GetStatusAsync(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .Where(x => x.Id == userId)
            .Select(s => s.Status)
            .FirstOrDefaultAsync();
    }

    public async Task<User> GetAndRegIfNoneAsync(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        var user = await dbContext.Users
            .Where(x => x.Id == userId)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            var newUser = new User()
            {
                Id = userId,
                GetMessages = true,
                Anon = true,
                Status = Status.Ok,
            };

            await dbContext.AddAsync(newUser);
            await dbContext.SaveChangesAsync();

            return newUser;
        }

        return user;
    }
}

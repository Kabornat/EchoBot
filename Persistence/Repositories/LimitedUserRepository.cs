using Microsoft.EntityFrameworkCore;
using Persistence.Models;

namespace Persistence.Repositories;

public class LimitedUserRepository(IDbContextFactory<AppDbContext> factory)
{
    private readonly IDbContextFactory<AppDbContext> _factory = factory;

    public async Task<DateTime> GetPeriodAsync(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.LimitedUsers
            .Where(x => x.UserId == userId)
            .Select(x => x.LimitPeriod)
            .FirstAsync();
    }

    public async Task<bool> MuteAsync(long userId, DateTime period)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await dbContext.Users
                .Where(x => x.Id == userId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Status, Status.Muted));

            if (await dbContext.LimitedUsers
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId))
            {
                await dbContext.LimitedUsers
                    .Where(x => x.UserId == userId)
                    .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.LimitPeriod, period));
            }
            else
            {
                var limitedUser = new LimitedUser()
                {
                    UserId = userId,
                    LimitPeriod = period,
                };

                await dbContext.AddAsync(limitedUser);
                await dbContext.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await transaction.RollbackAsync();
            return false;
        }

        return true;
    }

    public async Task<bool> UnmuteAsync(long userId, DateTime period)
    {
        if (period > DateTime.UtcNow)
            return false;

        await using var dbContext = await _factory.CreateDbContextAsync();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await dbContext.Users
                .Where(x => x.Id == userId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Status, Status.Ok));

            await dbContext.LimitedUsers
                .Where(x => x.UserId == userId)
                .ExecuteDeleteAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await transaction.RollbackAsync();
            return false;
        }

        return true;
    }

    public async Task<bool> UnmutePeriodReachedAsync()
    {
        var utcNow = DateTime.UtcNow;

        await using var dbContext = await _factory.CreateDbContextAsync();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await dbContext.LimitedUsers
                .Where(x => x.LimitPeriod <= utcNow &&
                        dbContext.Users
                        .Where(x => x.Status == Status.Muted)
                        .Select(x => x.Id)
                        .Contains(x.UserId))

                .ExecuteDeleteAsync();

            await dbContext.Users
                .Where(x => x.Status == Status.Muted &&
                        !dbContext.LimitedUsers
                        .Select(x => x.UserId)
                        .Contains(x.Id))

                .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Status, Status.Ok));

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await transaction.RollbackAsync();
            return false;
        }

        return true;
    }


    public async Task<bool> BanAsync(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.Status, Status.Banned)) > 0;
    }

    public async Task<bool> UnbanAsync(long userId)
    {
        await using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.Status, Status.Ok)) > 0;
    }
}

using System.Text;
using Microsoft.EntityFrameworkCore;
using Persistence.Models;
using Persistence.Repositories;

namespace Persistence.Services;

public class UserService(IDbContextFactory<AppDbContext> factory) : UserRepository(factory)
{
    private const int MAX_USERS = 20;

    public async Task<string> GetBanlistResponceAsync()
    {
        var userIds = await GetListAsync(Status.Banned);

        var sb = new StringBuilder(MAX_USERS)
            .AppendLine("<b>Список забаненых</b>\n");

        for (int i = 0; i < userIds.Count; i++)
        {
            if (i == MAX_USERS)
            {
                sb.Append("\n<b>Максимум элементов в списке!</b>");
                break;
            }

            sb.AppendLine($"<code>{userIds[i]}</code>");
        }

        return sb.ToString();
    }

    public async Task<string> GetAdminsResponceAsync()
    {
        var userIds = await GetListAsync(Status.Admin);

        var sb = new StringBuilder(MAX_USERS)
            .AppendLine("<b>Список админов</b>\n");

        for (int i = 0; i < userIds.Count; i++)
        {
            if (i == MAX_USERS)
            {
                sb.Append("\n<b>Максимум элементов в списке!</b>");
                break;
            }

            sb.AppendLine($"<code>{userIds[i]}</code>");
        }

        return sb.ToString();
    }

    public async Task<string> GetMutelistResponceAsync()
    {
        var userIds = await GetListAsync(Status.Muted);

        var sb = new StringBuilder(MAX_USERS)
            .AppendLine("<b>Список замученых</b>\n");

        for (int i = 0; i < userIds.Count; i++)
        {
            if (i == MAX_USERS)
            {
                sb.Append("\n<b>Максимум элементов в списке!</b>");
                break;
            }

            sb.AppendLine($"<code>{userIds[i]}</code>");
        }

        return sb.ToString();
    }
}

using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.Services;

public class LimitedUserService(IDbContextFactory<AppDbContext> factory) : LimitedUserRepository(factory)
{

}

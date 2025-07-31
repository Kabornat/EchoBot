using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.Services;

public class UserService(IDbContextFactory<AppDbContext> factory) : UserRepository(factory)
{

}

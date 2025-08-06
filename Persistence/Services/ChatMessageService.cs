using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.Services;

public class ChatMessageService(IDbContextFactory<AppDbContext> factory) : UserRepository(factory)
{

}

using Application;
using Application.Handlers;
using Application.Handlers.MessageHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.OwnerMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.UserMessageTextHandlers;
using Application.Services;
using EchoBot.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Persistence.OtherModels;
using Persistence.Repositories;
using Persistence.Services;
using Telegram.Bot;

// Настройка конфигурации
var environment = Environment.GetEnvironmentVariable("DOTNET_ECHO_ENVIRONMENT");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties"))
    .AddJsonFile($"appsettings.{environment}.json")
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnectionString");

// Регистрация сервисов
var services = new ServiceCollection();

// Persistence
services.AddDbContextFactory<AppDbContext>(provider =>
{
    provider.UseSqlite(connectionString);

    if (bool.TryParse(configuration["EFLogs"], out bool eFLogs) && eFLogs)
        provider.LogTo(Console.WriteLine);
});

services.AddSingleton<ChatMessageRepository>();
services.AddSingleton<LimitedUserRepository>();
services.AddSingleton<UserRepository>();

services.AddSingleton<ChatMessageService>();
services.AddSingleton<LimitedUserService>();
services.AddSingleton<UserService>();

services.AddSingleton<TimerManagerDelayWeek>();
services.AddSingleton<TimerManagerService>();

// Api
services.AddSingleton<CancellationTokenSource>();

services.AddSingleton<TelegramBotClient>(provider =>
{
    return new TelegramBotClient(configuration["BotToken"]);
});

services.AddSingleton<OwnerTextCommandsHandler>();
services.AddSingleton<AdminTextCommandsHandler>();
services.AddSingleton<UserTextCommandsHandler>();

services.AddSingleton<OwnerMessageTextHandler>();
services.AddSingleton<AdminMessageTextHandler>();
services.AddSingleton<UserMessageTextHandler>();

services.AddSingleton<MessageTextHandler>();
services.AddSingleton<MessageHandler>();

services.AddSingleton<UpdateHandler>();
services.AddSingleton<ErrorHandler>();

services.AddSingleton<EchoChatService>();

services.AddSingleton<CommandsList>();

services.AddSingleton<Startup>();
services.AddSingleton<Stop>();

// Other
services.AddSingleton<BotData>(provider =>
{
    var botData = configuration.GetRequiredSection("BotData").Get<BotData>();

    botData.UsernameWithDog = '@' + botData.Username;

    return botData;
});

services.AddSingleton<MainTextService>();

var serviceProvider = services.BuildServiceProvider();

// Запуск миграций

using (var scope = serviceProvider.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();

    var dbContext = factory.CreateDbContext();

    dbContext.Database.Migrate();
}

// Запуск проекта
serviceProvider.GetRequiredService<TimerManagerService>();

var startup = serviceProvider.GetRequiredService<Startup>();

await startup.RunAsync();
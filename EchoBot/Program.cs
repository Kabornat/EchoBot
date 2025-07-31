using Telegram.Bot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Handlers;
using Application.Handlers.MessageHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers;
using Application;
using Application.Handlers.MessageHandlers.MessageTextHandlers.PrivateChatMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using Persistence.Services;
using Application.Commands;
using Application.Services;

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

    provider.LogTo(Console.WriteLine);
});

services.AddSingleton<ChatMessageRepository>();
services.AddSingleton<LimitedUserRepository>();
services.AddSingleton<UserRepository>();

services.AddSingleton<ChatMessageService>();
services.AddSingleton<LimitedUserService>();
services.AddSingleton<UserService>();

// Api
services.AddSingleton<CancellationTokenSource>();

services.AddSingleton<TelegramBotClient>(provider =>
{
    return new TelegramBotClient(configuration["BotToken"]);
});

services.AddSingleton<CancellationTokenSource>();

services.AddSingleton<AdminMessageTextHandler>();
services.AddSingleton<PrivateChatMessageTextHandler>();

services.AddSingleton<MessageTextHandler>();
services.AddSingleton<MessageHandler>();

services.AddSingleton<UpdateHandler>();
services.AddSingleton<ErrorHandler>();

services.AddSingleton<SendMessageService>();
services.AddSingleton<BotCommands>();
services.AddSingleton<Startup>();

var serviceCollection = services.BuildServiceProvider();

// Запуск миграций

//using (var scope = serviceCollection.CreateScope())
//{
//    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();

//    var dbContext = factory.CreateDbContext();

//    dbContext.Database.Migrate();
//}

// Запуск проекта

var startup = serviceCollection.GetRequiredService<Startup>();

await startup.RunAsync();
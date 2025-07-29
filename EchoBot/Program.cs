using Telegram.Bot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Handlers;
using Application.Handlers.MessageHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers;
using Application;
using Application.Handlers.MessageHandlers.MessageTextHandlers.PrivateChatMessageTextHandlers;
using Application.Handlers.MessageHandlers.MessageTextHandlers.AdminMessageTextHandlers;

// Настройка конфигурации
var environment = Environment.GetEnvironmentVariable("DOTNET_ECHO_ENVIRONMENT");

var configurationBuilder = new ConfigurationBuilder();

var configuration = configurationBuilder
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties"))
    .AddJsonFile($"appsettings.{environment}.json")
    .Build();

// Регистрация сервисов
var services = new ServiceCollection();

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

services.AddSingleton<Startup>();

// Запуск проекта
var serviceCollection = services.BuildServiceProvider();

var startup = serviceCollection.GetRequiredService<Startup>();

await startup.RunAsync();
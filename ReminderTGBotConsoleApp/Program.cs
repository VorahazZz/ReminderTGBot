using System.Collections.Immutable;
using DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReminderTGBotConsoleApp.Interfaces;
using ReminderTGBotConsoleApp.Services;
using Telegram.Bot;

namespace ReminderTGBotConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory[..AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal)]);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                // Регистрируем Telegram Bot Client
                var botToken = context.Configuration["TelegramBot:Token"];
                services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken!)); // [!code ++]
                
                // 1. Регистрируем DbContext из проекта DataBase
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
        
                // 2. Регистрируем ваши сервисы
                services.AddSingleton<ITelegramBotService, TelegramBotService>();
        
                // 3. Регистрируем фоновый сервис из проекта Presentation
                services.AddHostedService<BotBackgroundService>();
            })
            .Build();

        // Применяем миграции при запуске
        using (var scope = host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        await host.RunAsync();
    }
}
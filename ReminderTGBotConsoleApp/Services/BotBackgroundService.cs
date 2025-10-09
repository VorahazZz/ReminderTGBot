using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReminderTGBotConsoleApp.Interfaces;

namespace ReminderTGBotConsoleApp.Services;

public class BotBackgroundService : BackgroundService
{
    private readonly ITelegramBotService _botService;
    private readonly ILogger<BotBackgroundService> _logger;

    public BotBackgroundService(ITelegramBotService botService, ILogger<BotBackgroundService> logger)
    {
        _botService = botService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BotBackgroundService запущен");

        await _botService.StartAsync(stoppingToken);

        _logger.LogInformation("BotBackgroundService остановлен");
    }
}
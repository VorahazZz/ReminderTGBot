using Microsoft.Extensions.Logging;
using ReminderTGBotConsoleApp.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ReminderTGBotConsoleApp.Services;

public class TelegramBotService : ITelegramBotService
{
    private readonly ILogger<TelegramBotService> _logger;

    public TelegramBotService(ILogger<TelegramBotService> logger)
    {
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("TelegramBotService запущен");

        await Task.CompletedTask;
    }
}
namespace ReminderTGBotConsoleApp.Interfaces;

public interface ITelegramBotService
{
    Task StartAsync(CancellationToken cancellationToken);
}
using Microsoft.Extensions.Logging;
using ReminderTGBotConsoleApp.Classes;
using ReminderTGBotConsoleApp.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace ReminderTGBotConsoleApp.Services;

public class TelegramBotService : ITelegramBotService
{
    private readonly ILogger<TelegramBotService> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private static readonly Dictionary<long, UserDialogState> _userStates = new();

    public TelegramBotService(ILogger<TelegramBotService> logger, ITelegramBotClient botClient, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _botClient = botClient;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            DropPendingUpdates = true
        };
        
        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken);

        var me = await _botClient.GetMe(cancellationToken);
        _logger.LogInformation($"Бот {me.Username} запущен");
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            await (update.Type switch
            {
                UpdateType.Message => OnMessageReceived(update.Message!, cancellationToken),
                UpdateType.CallbackQuery => OnCallbackQueryReceived(update.CallbackQuery!, cancellationToken),
                _ => Task.CompletedTask
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при обработки обновления: {ex}");
            throw;
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        
        _logger.LogError(errorMessage);
        return Task.CompletedTask;
    }
    
    private async Task OnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        var userId = message.From!.Id;
        
        _logger.LogInformation($"Получено сообщение '{messageText}' в чате '{chatId}'");

        if (messageText.StartsWith('/'))
        {
            await HandleCommand(message, cancellationToken);
            return;
        }

        await HandleTextMessage(message, cancellationToken);
    }

    private async Task HandleTextMessage(Message message, CancellationToken cancellationToken)
    {
        // TODO: Проверить состояние диалога и обработать ввод соответственно
        // Если ожидаем название лекарства → сохранить и запросить дни
        // Если ожидаем время → сохранить и показать подтверждение
    }

    private async Task HandleCommand(Message message, CancellationToken cancellationToken)
    {
        var command = message.Text.Split(' ')[0].ToLower();
        var chatId = message.Chat.Id;

        switch(command)
        {
            case "/start":
                await SendWelcomeMessage(chatId, cancellationToken);
                break;
            case "/new":
                await StartNewReminderCreation(chatId, cancellationToken);
                break;
            default:
                await _botClient.SendMessage(
                    chatId: chatId,
                    text: "Неизвестная команда. Используйте /start для начала работы.",
                    cancellationToken: cancellationToken);
                break;
        }
    }
    
    private async Task SendWelcomeMessage(long chatId, CancellationToken cancellationToken)
    {
        var welcomeMessage = """
                             💊 **Привет! Я бот для напоминаний о приеме лекарств**

                             Вот что я умею:
                             • Создавать напоминания о приеме лекарств
                             • Показывать список ваших напоминаний
                             • Включать/выключать напоминания

                             Используйте команды:
                             /new - создать новое напоминание
                             /list - показать список напоминаний

                             Начните с команды /new чтобы создать первое напоминание!
                             """;

        await _botClient.SendMessage(
            chatId: chatId,
            text: welcomeMessage,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }

    private async Task StartNewReminderCreation(long chatId, CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(
            chatId: chatId,
            text: "💊 **Создание нового напоминания**\n\nНапишите название лекарства:",
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
        
        // TODO: Сохранить состояние, что ожидаем ввод названия лекарства
    }

    private async Task OnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        if (callbackQuery.Data is not { } data)
            return;

        var chatId = callbackQuery.Message.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        
        _logger.LogInformation($"Получен callback: {data} в чате {chatId}");

        if (data.StartsWith("day_"))
        {
            await HandleDaySelection(callbackQuery, data, cancellationToken);
        }
        else if (data == "confirm_reminder")
        {
            await HandleReminderConfirmation(chatId, cancellationToken);
        }

        await _botClient.AnswerCallbackQuery(
            callbackQueryId: callbackQuery.Id,
            cancellationToken: cancellationToken);
    }

    private async Task HandleReminderConfirmation(long chatId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task HandleDaySelection(CallbackQuery callbackQuery, string data, CancellationToken cancellationToken)
    {
        // TODO: Реализовать логику выбора дней недели
        // Будем обновлять сообщение с новым состоянием выбранных дней
        throw new NotImplementedException();
    }
}
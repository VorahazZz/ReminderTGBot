namespace ReminderTGBotConsoleApp.Classes;

public class UserDialogState
{
    public string CurrentStep { get; set; } = string.Empty;
    public string? DrugName { get; set; }
    public List<string> SelectedDays { get; set; } = new();
    public string? ReminderTime { get; set; }
}
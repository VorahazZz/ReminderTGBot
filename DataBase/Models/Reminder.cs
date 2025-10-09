namespace DataBase.Models;

public class Reminder
{
    public int Id { get; set; }
    public string DrugName { get; set; }
    public string DaysOfWeek { get; set; }
    public string ReminderTime { get; set; }
    public bool IsDisabled { get; set; }
    public long UserId { get; set; }
    public virtual User User { get; set; }
}
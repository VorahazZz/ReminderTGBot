namespace DataBase.Models;

public class User
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
    
    // Навигационное свойство
    public virtual ICollection<Reminder> Reminders { get; set; }
}
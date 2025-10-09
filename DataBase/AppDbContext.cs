using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Reminder> Reminders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Настройка таблицы Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId); // Первичный ключ
            entity.HasIndex(u => u.ChatId).IsUnique(); // Уникальный индекс для ChatId
                
            // Свойства
            entity.Property(u => u.UserId)
                .ValueGeneratedNever(); // Не генерировать автоматически, т.к. берем из Telegram
                
            entity.Property(u => u.ChatId)
                .IsRequired();
        });
        
        // Настройка таблицы Reminders
        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(r => r.Id); // Первичный ключ
                
            // Свойства
            entity.Property(r => r.Id)
                .ValueGeneratedOnAdd(); // Автоинкремент
                
            entity.Property(r => r.DrugName)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(r => r.DaysOfWeek)
                .IsRequired()
                .HasMaxLength(50); // "1,3,5" для Пн,Ср,Пт
                
            entity.Property(r => r.ReminderTime)
                .IsRequired()
                .HasMaxLength(5); // "09:00"
                
            entity.Property(r => r.IsDisabled)
                .IsRequired()
                .HasDefaultValue(false);

            // Внешний ключ к Users
            entity.HasOne(r => r.User)
                .WithMany() // У пользователя много напоминаний
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении пользователя удаляем его напоминания
        });
    }
}
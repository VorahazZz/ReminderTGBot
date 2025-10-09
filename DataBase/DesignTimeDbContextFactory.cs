using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataBase;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Настройка конфигурации для чтения строки подключения из appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Текущая директория проекта DataBase
            .AddJsonFile("appsettings.json")
            .Build();
            
        var connectionString = configuration.GetConnectionString("DefaultConnection");
            
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseNpgsql(connectionString); // Используем PostgreSQL
            
        return new AppDbContext(builder.Options);
    }
}
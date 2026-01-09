using System.Text.Json;
using AllulExpressApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class MySqlDbLoggingInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;

    public MySqlDbLoggingInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        Console.WriteLine("DB LOG INTERCEPTOR HIT");

        WriteLogs(eventData.Context);
        return result;
    }

    private void WriteLogs(DbContext context)
    {
        Console.WriteLine("DB LOG INTERCEPTOR HIT");

        if (context == null) return;

        var logs = new List<DbLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is DbLog) continue;

            if (entry.State == EntityState.Added ||
                entry.State == EntityState.Modified ||
                entry.State == EntityState.Deleted)
            {
                logs.Add(new DbLog
                {
                    TableName = entry.Metadata.GetTableName(),
                    Action = entry.State.ToString().ToUpper(),
                    KeyValues = JsonSerializer.Serialize(
                        entry.Properties
                            .Where(p => p.Metadata.IsPrimaryKey())
                            .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue)
                    ),
                    OldValues = entry.State == EntityState.Modified
                        ? JsonSerializer.Serialize(
                            entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue))
                        : null,
                    NewValues = entry.State != EntityState.Deleted
                        ? JsonSerializer.Serialize(
                            entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue))
                        : null,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        if (!logs.Any()) return;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.DbLogs.AddRange(logs);
        db.SaveChanges();
    }
}

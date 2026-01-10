using System.Text.Json;
using AllulExpressApi.Data;
using AllulExpressApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class MySqlDbLoggingInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;
    private List<DbLog> _pendingLogs = new();

    public MySqlDbLoggingInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null) return result;

        _pendingLogs.Clear();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // ⛔ skip logging tables
            if (entry.Entity is DbLog || entry.Entity is ValidToken)
                continue;

            if (entry.State == EntityState.Added ||
                entry.State == EntityState.Modified ||
                entry.State == EntityState.Deleted)
            {
                _pendingLogs.Add(new DbLog
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

        return result;
    }

    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result)
    {
        if (!_pendingLogs.Any())
            return result;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.DbLogs.AddRange(_pendingLogs);
        db.SaveChanges();   // ✅ SAFE HERE

        _pendingLogs.Clear();
        return result;
    }
}

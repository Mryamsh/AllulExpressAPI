using System.Text.Json;
using System.Runtime.CompilerServices;
using AllulExpressApi.Data;
using AllulExpressApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class MySqlDbLoggingInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;

    // ✅ SAFE storage per DbContext
    private static readonly ConditionalWeakTable<DbContext, List<DbLog>> _logsTable
        = new();

    public MySqlDbLoggingInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        CollectLogs(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        CollectLogs(eventData.Context);
        return ValueTask.FromResult(result);
    }

    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result)
    {
        PersistLogs(eventData.Context);
        return result;
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        PersistLogs(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void CollectLogs(DbContext? context)
    {

        Console.WriteLine("here log");
        if (context == null) return;

        var logs = new List<DbLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is DbLog || entry.Entity is ValidToken)
                continue;

            if (entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            {
                logs.Add(new DbLog
                {
                    TableName = entry.Metadata.GetTableName(),
                    Action = entry.State.ToString().ToUpper(),

                    UserId = context is AppDbContext appDb
                    ? appDb.CurrentUserId
                     : null,
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

        if (logs.Count > 0)
        {
            _logsTable.Remove(context); // prevent duplicates
            _logsTable.Add(context, logs);
        }
    }

    private void PersistLogs(DbContext? context)
    {
        if (context == null) return;
        if (!_logsTable.TryGetValue(context, out var logs)) return;
        if (logs.Count == 0) return;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.DbLogs.AddRange(logs);
        db.SaveChanges();

        _logsTable.Remove(context);
    }
}

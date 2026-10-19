using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Gobln.Pager.EntityFrameworkCoreTest;

public sealed class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
}

public sealed class ItemContext(DbContextOptions<ItemContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();
}

/// <summary>
/// Records every query EF Core sends, and whether it was sent asynchronously
/// </summary>
public sealed class CommandRecorder : DbCommandInterceptor
{
    public List<(string Sql, bool IsAsync)> Commands { get; } = [];

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        Commands.Add((command.CommandText, false));
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        Commands.Add((command.CommandText, true));
        return ValueTask.FromResult(result);
    }

    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        Commands.Add((command.CommandText, false));
        return result;
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
    {
        Commands.Add((command.CommandText, true));
        return ValueTask.FromResult(result);
    }
}

/// <summary>
/// An in-memory SQLite database holding "Item 1" to "Item N", with a recorder that starts out empty
/// </summary>
public sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public TestDatabase(int itemCount = 25)
    {
        // The in-memory database lives as long as this connection is open
        _connection.Open();

        Context = new ItemContext(new DbContextOptionsBuilder<ItemContext>()
            .UseSqlite(_connection)
            .AddInterceptors(Recorder)
            .Options);

        Context.Database.EnsureCreated();
        Context.Items.AddRange(Enumerable.Range(1, itemCount).Select(i => new Item { Id = i, Name = $"Item {i}" }));
        Context.SaveChanges();
        Context.ChangeTracker.Clear();

        Recorder.Commands.Clear();
    }

    public CommandRecorder Recorder { get; } = new();

    public ItemContext Context { get; }

    /// <summary>
    /// The items, sorted by Id
    /// </summary>
    public IQueryable<Item> Items => Context.Items.OrderBy(i => i.Id);

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}

using System.Collections;
using System.Linq.Expressions;

namespace Gobln.PagerTest;

/// <summary>
/// A LINQ to Objects query that records what runs, to tell paging in the query from paging in memory
/// </summary>
internal static class RecordingQueryable
{
    public static IQueryable<T> From<T>(IEnumerable<T> items, out RecordingQueryProvider provider)
    {
        var inner = items.AsQueryable();
        provider = new RecordingQueryProvider(inner.Provider);

        return new RecordingQueryable<T>(provider, inner.Expression);
    }
}

internal sealed class RecordingQueryable<T>(RecordingQueryProvider provider, Expression expression) : IQueryable<T>
{
    public Type ElementType => typeof(T);

    public Expression Expression => expression;

    public IQueryProvider Provider => provider;

    public IEnumerator<T> GetEnumerator() => provider.Enumerate<T>(expression);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class RecordingQueryProvider(IQueryProvider inner) : IQueryProvider
{
    /// <summary>
    /// Queries run for a single value, such as a Count
    /// </summary>
    public List<Expression> Executed { get; } = [];

    /// <summary>
    /// Queries whose items were read
    /// </summary>
    public List<Expression> Enumerated { get; } = [];

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new RecordingQueryable<TElement>(this, expression);

    public IQueryable CreateQuery(Expression expression) => throw new NotSupportedException();

    public TResult Execute<TResult>(Expression expression)
    {
        Executed.Add(expression);
        return inner.Execute<TResult>(expression);
    }

    public object? Execute(Expression expression) => throw new NotSupportedException();

    public IEnumerator<T> Enumerate<T>(Expression expression)
    {
        Enumerated.Add(expression);
        return inner.CreateQuery<T>(expression).GetEnumerator();
    }
}

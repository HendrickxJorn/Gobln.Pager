using System.Linq.Expressions;
using Gobln.Pager;
using Xunit;

namespace Gobln.PagerTest;

/// <summary>
/// ToPageAsync on a query, without Gobln.Pager.EntityFrameworkCore: paged in the query, on the thread pool
/// </summary>
public class PageAsyncQueryTest
{
    private static readonly int[] Numbers = [.. Enumerable.Range(1, 25)];

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ToPageAsync_OnAQuery_CountsAndPagesInTheQuery(bool withFilter)
    {
        var query = RecordingQueryable.From(Numbers, out var provider);

        var page = withFilter
            ? await query.ToPageAsync(new PageFilter(2, 10))
            : await query.ToPageAsync(2, 10);

        Assert.Equal(Enumerable.Range(11, 10), page);
        Assert.Equal(25, page.TotalItemCount);
        Assert.Equal(new[] { "Count" }, MethodNames(Assert.Single(provider.Executed)));
        Assert.Equal(new[] { "Take", "Skip" }, MethodNames(Assert.Single(provider.Enumerated)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ToPageAsync_OnAQuery_WithATotal_PagesInTheQuery(bool withFilter)
    {
        var query = RecordingQueryable.From(Numbers, out var provider);

        var page = withFilter
            ? await query.ToPageAsync(new PageFilter(3, 10), 25, prePaged: false)
            : await query.ToPageAsync(3, 10, 25, prePaged: false);

        Assert.Equal(Enumerable.Range(21, 5), page);
        Assert.Empty(provider.Executed);
        Assert.Equal(new[] { "Take", "Skip" }, MethodNames(Assert.Single(provider.Enumerated)));
    }

    // The query operators of an expression, outermost first: Take(Skip(source)) gives Take, Skip
    private static List<string> MethodNames(Expression expression)
    {
        var names = new List<string>();

        while (expression is MethodCallExpression call)
        {
            names.Add(call.Method.Name);
            expression = call.Arguments[0];
        }

        return names;
    }
}

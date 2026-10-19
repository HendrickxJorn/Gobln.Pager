using Xunit;

namespace Gobln.Pager.EntityFrameworkCoreTest;

// This namespace sits inside Gobln.Pager, so the core's and the package's extensions are found
// in the same scope, as in an app with `using Gobln.Pager;`. The package's methods have to win:
// if the core's were picked, the queries below would run synchronously.
public class ToPageAsyncTest
{
    [Fact]
    public async Task ToPageAsync_RunsTheCountAndThePageQueryAsynchronously()
    {
        using var db = new TestDatabase();

        await db.Items.ToPageAsync(2, 10);

        Assert.Equal(2, db.Recorder.Commands.Count);
        Assert.All(db.Recorder.Commands, c => Assert.True(c.IsAsync, c.Sql));
    }

    [Fact]
    public async Task ToPageAsync_FetchesOnlyThePage()
    {
        using var db = new TestDatabase();

        var page = await db.Items.ToPageAsync(2, 10);

        Assert.Equal(Enumerable.Range(11, 10), page.Select(i => i.Id));
        Assert.Equal(2, page.PageIndex);
        Assert.Equal(10, page.PageSize);
        Assert.Equal(25, page.TotalItemCount);
        Assert.Equal(3, page.TotalPageCount);
        Assert.Contains("LIMIT", db.Recorder.Commands[^1].Sql);
    }

    [Fact]
    public async Task ToPageAsync_PastTheLastPage_ReturnsTheLastPage()
    {
        using var db = new TestDatabase();

        var page = await db.Items.ToPageAsync(9, 10);

        Assert.Equal(3, page.PageIndex);
        Assert.Equal(Enumerable.Range(21, 5), page.Select(i => i.Id));
    }

    [Fact]
    public async Task ToPageAsync_EmptyTable_RunsOnlyTheCount()
    {
        using var db = new TestDatabase(itemCount: 0);

        var page = await db.Items.ToPageAsync(2, 10);

        Assert.True(page.IsEmpty);
        Assert.Equal(1, page.PageIndex);
        Assert.Equal(0, page.TotalItemCount);
        Assert.Equal(1, page.TotalPageCount);
        Assert.Single(db.Recorder.Commands);
    }

    [Fact]
    public async Task ToPageAsync_PrePaged_RunsOnlyTheItemsQuery()
    {
        using var db = new TestDatabase();

        // The caller paged and counted already: items 11 to 20 of 95
        var page = await db.Items.Skip(10).Take(10).ToPageAsync(2, 10, 95);

        Assert.Equal(Enumerable.Range(11, 10), page.Select(i => i.Id));
        Assert.Equal(95, page.TotalItemCount);
        Assert.Equal(10, page.TotalPageCount);
        var command = Assert.Single(db.Recorder.Commands);
        Assert.True(command.IsAsync);
        Assert.DoesNotContain("COUNT", command.Sql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ToPageAsync_PrePaged_WithNoItems_RunsNoQuery()
    {
        using var db = new TestDatabase();

        var page = await db.Items.ToPageAsync(1, 10, 0);

        Assert.True(page.IsEmpty);
        Assert.Equal(0, page.TotalItemCount);
        Assert.Empty(db.Recorder.Commands);
    }

    [Fact]
    public async Task ToPageAsync_NotPrePaged_WithATotal_RunsOnlyThePageQuery()
    {
        using var db = new TestDatabase();

        var page = await db.Items.ToPageAsync(3, 10, 25, prePaged: false);

        Assert.Equal(Enumerable.Range(21, 5), page.Select(i => i.Id));
        var command = Assert.Single(db.Recorder.Commands);
        Assert.True(command.IsAsync);
        Assert.Contains("LIMIT", command.Sql);
        Assert.DoesNotContain("COUNT", command.Sql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ToPageAsync_WithAFilter_Pages()
    {
        using var db = new TestDatabase();
        var filter = new PageFilter(2, 10);

        var counted = await db.Items.ToPageAsync(filter);
        var prePaged = await db.Items.Skip(10).Take(10).ToPageAsync(filter, 25);
        var notPrePaged = await db.Items.ToPageAsync(filter, 25, prePaged: false);

        Assert.All(new[] { counted, prePaged, notPrePaged }, page => Assert.Equal(Enumerable.Range(11, 10), page.Select(i => i.Id)));
        // A count and a page, then one query each
        Assert.Equal(4, db.Recorder.Commands.Count);
        Assert.All(db.Recorder.Commands, c => Assert.True(c.IsAsync, c.Sql));
    }

    [Fact]
    public async Task ToPageAsync_WithoutArguments_PutsEverythingOnOnePage()
    {
        using var db = new TestDatabase();

        var page = await db.Items.ToPageAsync();

        Assert.Equal(25, page.Count);
        Assert.Equal(1, page.PageIndex);
        Assert.Equal(25, page.PageSize);
        Assert.Equal(1, page.TotalPageCount);
        var command = Assert.Single(db.Recorder.Commands);
        Assert.True(command.IsAsync);
    }

    [Fact]
    public async Task ToPagedListAsync_ReadsTheQueryAsynchronously()
    {
        using var db = new TestDatabase();

        var byTen = await db.Items.ToPagedListAsync();
        var bySeven = await db.Items.ToPagedListAsync(7);

        Assert.Equal(25, byTen.Count);
        Assert.Equal(3, byTen.PageCount);
        Assert.Equal(4, bySeven.PageCount);
        Assert.Equal(2, db.Recorder.Commands.Count);
        Assert.All(db.Recorder.Commands, c => Assert.True(c.IsAsync, c.Sql));
    }

    [Fact]
    public async Task ToPageAsync_OnANonEfQuery_FallsBackToTheCore()
    {
        var query = Enumerable.Range(1, 25).AsQueryable();

        var page = await query.ToPageAsync(2, 10);
        var list = await query.ToPagedListAsync(5);

        Assert.Equal(Enumerable.Range(11, 10), page);
        Assert.Equal(25, page.TotalItemCount);
        Assert.Equal(5, list.PageCount);
    }

    [Fact]
    public async Task ToPageAsync_Cancelled_Throws()
    {
        using var db = new TestDatabase();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => db.Items.ToPageAsync(1, 10, cancellation.Token));
        Assert.DoesNotContain(db.Recorder.Commands, c => !c.IsAsync);
    }

    [Fact]
    public async Task ToPageAsync_InvalidInput_ThrowsBeforeQuerying()
    {
        using var db = new TestDatabase();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => db.Items.ToPageAsync(0, 10));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => db.Items.ToPageAsync(1, 0));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => db.Items.ToPageAsync(1, 10, -1));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => db.Items.ToPagedListAsync(0));
        Assert.Empty(db.Recorder.Commands);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(3, 10)]
    [InlineData(9, 10)]
    [InlineData(2, 25)]
    [InlineData(1, 100)]
    public async Task ToPageAsync_MatchesTheSyncToPage(int pageIndex, int pageSize)
    {
        using var db = new TestDatabase();

        var expected = db.Items.ToPage(pageIndex, pageSize);
        var actual = await db.Items.ToPageAsync(pageIndex, pageSize);

        AssertSamePage(expected, actual);
    }

    private static void AssertSamePage(Page<Item> expected, Page<Item> actual)
    {
        Assert.Equal(expected.PageIndex, actual.PageIndex);
        Assert.Equal(expected.PageSize, actual.PageSize);
        Assert.Equal(expected.TotalItemCount, actual.TotalItemCount);
        Assert.Equal(expected.TotalPageCount, actual.TotalPageCount);
        Assert.Equal(expected.Select(i => i.Id), actual.Select(i => i.Id));
    }
}

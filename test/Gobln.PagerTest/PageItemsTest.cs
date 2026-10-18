using Gobln.Pager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Gobln.PagerTest;

/// <summary>
/// A <see cref="Page{T}"/> holds its own items: the source is read while the page is made,
/// and never again when the page is enumerated or checked. For a query that means no extra
/// round trips, and a page that still works after its DbContext is gone.
/// </summary>
public class PageItemsTest
{
    /// <summary>
    /// The numbers 1 to <paramref name="count"/>, counting how often they are enumerated
    /// </summary>
    private sealed class CountingSource(int count) : IEnumerable<int>
    {
        public int Enumerations { get; private set; }

        public IEnumerator<int> GetEnumerator()
        {
            Enumerations++;
            return Enumerable.Range(1, count).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Fact]
    public void EnumerablePage_DoesNotReadTheSourceAgain()
    {
        var source = new CountingSource(30);

        var page = source.ToPage(2, 10);
        var readsToMakeThePage = source.Enumerations;

        _ = page.ToList();
        _ = page.ToList();
        _ = page.IsEmpty;

        Assert.Equal(readsToMakeThePage, source.Enumerations);
        Assert.Equal(Enumerable.Range(11, 10), page);
    }

    [Fact]
    public void QueryablePage_DoesNotRunTheQueryAgain()
    {
        var source = new CountingSource(30);

        var page = source.AsQueryable().ToPage(2, 10);

        // One query for the count, one for the items of the page
        Assert.Equal(2, source.Enumerations);

        _ = page.ToList();
        _ = page.ToList();
        _ = page.IsEmpty;

        Assert.Equal(2, source.Enumerations);
        Assert.Equal(Enumerable.Range(11, 10), page);
    }
}

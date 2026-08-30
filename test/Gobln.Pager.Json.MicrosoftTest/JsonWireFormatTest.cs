using Gobln.Pager.Json.Microsoft;
using System.Text.Json;
using Xunit;

namespace Gobln.Pager.Json.MicrosoftTest;

/// <summary>
/// Pins the JSON wire format produced by this package.
///
/// The shape asserted here is a contract shared with Gobln.Pager.Json.Newtonsoft:
/// Gobln.Pager.Json.NewtonsoftTest.JsonWireFormatTest asserts the exact same literals,
/// so a drift in either package fails that package's own test. Keep the two in sync --
/// changing them is a breaking change for anything that already stores or exchanges
/// this format.
///
/// These use Page&lt;int&gt;/PagedList&lt;int&gt; deliberately: an element type with no
/// DateTime or nullable members keeps the assertion about the page envelope rather
/// than about how each serializer happens to format values.
/// </summary>
public class JsonWireFormatTest
{
    private const string PageJson =
        """{"PageItems":[1,2,3],"PageIndex":2,"PageSize":3,"TotalItemCount":42}""";

    private const string PagedListJson = "[1,2,3,4,5]";

    private static JsonSerializerOptions Options() => new()
    {
        Converters =
        {
            new PageJsonConverter(),
            new PagedListJsonConverter()
        }
    };

    [Fact]
    public void Serialize_Page_WritesBareObject()
    {
        var page = new[] { 1, 2, 3 }.ToPage(2, 3, 42);

        Assert.Equal(PageJson, JsonSerializer.Serialize(page, Options()));
    }

    [Fact]
    public void Deserialize_Page_RestoresItemsAndMetadata()
    {
        var page = JsonSerializer.Deserialize<Page<int>>(PageJson, Options());

        Assert.NotNull(page);
        Assert.Equal(2, page.PageIndex);
        Assert.Equal(3, page.PageSize);
        Assert.Equal(42, page.TotalItemCount);
        Assert.Equal(14, page.TotalPageCount);
        Assert.Equal([1, 2, 3], page);
    }

    [Fact]
    public void RoundTrip_Page_PreservesMetadata()
    {
        var page = new[] { 1, 2, 3 }.ToPage(2, 3, 42);

        var result = JsonSerializer.Deserialize<Page<int>>(
            JsonSerializer.Serialize(page, Options()), Options());

        Assert.NotNull(result);
        Assert.Equal(page.PageIndex, result.PageIndex);
        Assert.Equal(page.PageSize, result.PageSize);
        Assert.Equal(page.TotalItemCount, result.TotalItemCount);
        Assert.Equal(page.TotalPageCount, result.TotalPageCount);
        Assert.Equal(page, result);
    }

    [Fact]
    public void Deserialize_ArrayWrappedPage_IsRejected()
    {
        // Before 1.0 this package wrapped the page object in a one-element array, which
        // Gobln.Pager.Json.Newtonsoft never did. That format is no longer written, and
        // must fail loudly rather than half-parse.
        var legacyJson = $"[{PageJson}]";

        Assert.Throws<JsonException>(
            () => JsonSerializer.Deserialize<Page<int>>(legacyJson, Options()));
    }

    [Fact]
    public void Serialize_PagedList_WritesPlainArray()
    {
        var pagedList = new[] { 1, 2, 3, 4, 5 }.ToPagedList(2);

        Assert.Equal(PagedListJson, JsonSerializer.Serialize(pagedList, Options()));
    }

    [Fact]
    public void RoundTrip_PagedList_PreservesItems()
    {
        var result = JsonSerializer.Deserialize<PagedList<int>>(PagedListJson, Options());

        Assert.NotNull(result);
        Assert.Equal([1, 2, 3, 4, 5], result);
    }
}

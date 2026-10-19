using Gobln.Pager.Json.Newtonsoft;
using Newtonsoft.Json;
using Xunit;

namespace Gobln.Pager.Json.NewtonsoftTest;

/// <summary>
/// Pins the JSON wire format produced by this package.
///
/// The shape asserted here is a contract shared with Gobln.Pager.Json.Microsoft:
/// Gobln.Pager.Json.MicrosoftTest.JsonWireFormatTest asserts the exact same literals,
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

    private static JsonSerializerSettings Options() => new()
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

        Assert.Equal(PageJson, JsonConvert.SerializeObject(page, Options()));
    }

    [Fact]
    public void Deserialize_Page_RestoresItemsAndMetadata()
    {
        var page = JsonConvert.DeserializeObject<Page<int>>(PageJson, Options());

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

        var result = JsonConvert.DeserializeObject<Page<int>>(
            JsonConvert.SerializeObject(page, Options()), Options());

        Assert.NotNull(result);
        Assert.Equal(page.PageIndex, result.PageIndex);
        Assert.Equal(page.PageSize, result.PageSize);
        Assert.Equal(page.TotalItemCount, result.TotalItemCount);
        Assert.Equal(page.TotalPageCount, result.TotalPageCount);
        Assert.Equal(page, result);
    }

    [Theory]
    [InlineData("""{"PageItems":[1],"PageIndex":0,"PageSize":10,"TotalItemCount":1}""")]
    [InlineData("""{"PageItems":[1],"PageIndex":1,"PageSize":0,"TotalItemCount":1}""")]
    [InlineData("""{"PageItems":[1],"PageIndex":1,"PageSize":10,"TotalItemCount":-1}""")]
    public void Deserialize_PageWithInvalidValues_IsRejectedAsInvalidJson(string json)
    {
        // The JSON is client input: values out of range make it invalid JSON, not an
        // argument error from inside the converter
        var exception = Assert.Throws<JsonSerializationException>(
            () => JsonConvert.DeserializeObject<Page<int>>(json, Options()));

        Assert.IsType<ArgumentOutOfRangeException>(exception.InnerException);
    }

    [Fact]
    public void Serialize_PagedList_WritesPlainArray()
    {
        var pagedList = new[] { 1, 2, 3, 4, 5 }.ToPagedList(2);

        Assert.Equal(PagedListJson, JsonConvert.SerializeObject(pagedList, Options()));
    }

    [Fact]
    public void RoundTrip_PagedList_PreservesItems()
    {
        var result = JsonConvert.DeserializeObject<PagedList<int>>(PagedListJson, Options());

        Assert.NotNull(result);
        Assert.Equal([1, 2, 3, 4, 5], result);
    }

    [Fact]
    public void RoundTrip_PageWithoutItems_KeepsItsTotals()
    {
        // A pre-paged page that came back empty still tells a pager how many pages there are
        var page = Array.Empty<int>().ToPage(3, 10, 25);

        var result = JsonConvert.DeserializeObject<Page<int>>(
            JsonConvert.SerializeObject(page, Options()), Options());

        Assert.NotNull(result);
        Assert.Empty(result);
        Assert.Equal(3, result.PageIndex);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalItemCount);
        Assert.Equal(3, result.TotalPageCount);
    }

    [Fact]
    public void Deserialize_EmptyPageWithoutTotals_IsAnEmptyPage()
    {
        // What new Page<T>() writes. Its PageSize of 0 must not be rejected as invalid.
        var result = JsonConvert.DeserializeObject<Page<int>>(
            """{"PageItems":[],"PageIndex":1,"PageSize":0,"TotalItemCount":0}""", Options());

        Assert.NotNull(result);
        Assert.True(result.IsEmpty);
        Assert.Equal(0, result.TotalItemCount);
        Assert.Equal(1, result.TotalPageCount);
    }
}

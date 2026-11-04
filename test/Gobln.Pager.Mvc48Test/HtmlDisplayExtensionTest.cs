using Gobln.Pager.Mvc;
using Gobln.Pager.Mvc48Test.Helper;
using System.Web.Mvc;
using Xunit;

namespace Gobln.Pager.Mvc48Test;

/// <summary>
/// DescriptionFor and DisplayNameFor on .NET Framework. Both are declared on
/// HtmlHelper&lt;Page&lt;T&gt;&gt; and HtmlHelper&lt;PagedList&lt;T&gt;&gt;, resolve the expression
/// against the element type, and return HTML-encoded text.
/// </summary>
public class HtmlDisplayExtensionTest
{
    private readonly HelperHtml _helperHtml = new();

    // The view's model is a page, so its view data starts without one: the default "Test" isn't a page
    private HtmlHelper<TModel> CreateHelper<TModel>() => _helperHtml.CreateHtmlHelper<TModel>(new ViewDataDictionary());

    #region DescriptionFor

    [Fact]
    public void DescriptionFor_ReadsDescriptionAttribute()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("From DescriptionAttribute", html.DescriptionFor(m => m.WithDescription).ToHtmlString());
    }

    [Fact]
    public void DescriptionFor_FallsBackToDisplayAttribute()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("From DisplayAttribute", html.DescriptionFor(m => m.WithDisplayDescription).ToHtmlString());
    }

    [Fact]
    public void DescriptionFor_PrefersDescriptionAttributeOverDisplayAttribute()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("DescriptionAttribute wins", html.DescriptionFor(m => m.WithBoth).ToHtmlString());
    }

    [Fact]
    public void DescriptionFor_WithoutEitherAttribute_IsEmpty()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("", html.DescriptionFor(m => m.Plain).ToHtmlString());
    }

    [Fact]
    public void DescriptionFor_IsHtmlEncoded()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("&lt;b&gt;Tom &amp; Jerry&lt;/b&gt;", html.DescriptionFor(m => m.WithMarkup).ToHtmlString());
    }

    [Fact]
    public void DescriptionFor_WorksOnPagedListModel()
    {
        var html = CreateHelper<PagedList<DescribedModel>>();

        Assert.Equal("From DescriptionAttribute", html.DescriptionFor(m => m.WithDescription).ToHtmlString());
    }

    [Fact]
    public void DescriptionFor_NullExpression_Throws()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Throws<ArgumentNullException>(
            () => html.DescriptionFor<DescribedModel, string?>(null!));
    }

    #endregion DescriptionFor

    #region DisplayNameFor

    [Fact]
    public void DisplayNameFor_ReadsTheDisplayName()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("Friendly name", html.DisplayNameFor(m => m.WithDisplayName).ToHtmlString());
    }

    [Fact]
    public void DisplayNameFor_WithoutADisplayName_IsThePropertyName()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("Plain", html.DisplayNameFor(m => m.Plain).ToHtmlString());
    }

    [Fact]
    public void DisplayNameFor_IsHtmlEncoded()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("&lt;b&gt;Tom &amp; Jerry&lt;/b&gt;", html.DisplayNameFor(m => m.WithMarkup).ToHtmlString());
    }

    [Fact]
    public void DisplayNameFor_WorksOnPagedListModel()
    {
        var html = CreateHelper<PagedList<DescribedModel>>();

        Assert.Equal("Friendly name", html.DisplayNameFor(m => m.WithDisplayName).ToHtmlString());
    }

    [Fact]
    public void DisplayNameFor_NullExpression_Throws()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Throws<ArgumentNullException>(
            () => html.DisplayNameFor<DescribedModel, string?>(null!));
    }

    #endregion DisplayNameFor
}

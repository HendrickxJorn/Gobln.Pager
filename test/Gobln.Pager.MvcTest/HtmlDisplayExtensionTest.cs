using Gobln.Pager.Mvc;
using Gobln.Pager.MvcTest.Helper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace Gobln.Pager.MvcTest;

/// <summary>
/// DescriptionFor and DisplayNameFor. Both are declared on IHtmlHelper&lt;Page&lt;T&gt;&gt;
/// and IHtmlHelper&lt;PagedList&lt;T&gt;&gt;, and the point of both is that the expression
/// resolves against the *element* type rather than the page wrapper.
/// </summary>
public class HtmlDisplayExtensionTest
{
    /// <summary>
    /// A metadata provider that honours data annotations, so [Display(Name = ...)] is
    /// resolved the way it would be in a real view.
    /// </summary>
    private static IModelMetadataProvider MetadataProvider { get; } =
        new ServiceCollection()
            .AddMvcCore()
            .AddDataAnnotations()
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IModelMetadataProvider>();

    private static IHtmlHelper<TModel> CreateHelper<TModel>()
    {
        var mock = new Mock<IHtmlHelper<TModel>>();

        mock.Setup(h => h.MetadataProvider).Returns(MetadataProvider);
        mock.Setup(h => h.ViewData).Returns(
            new ViewDataDictionary<TModel>(MetadataProvider, new ModelStateDictionary()));

        return mock.Object;
    }

    #region DescriptionFor

    [Fact]
    public void DescriptionFor_ReadsDescriptionAttribute()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("From DescriptionAttribute", html.DescriptionFor(m => m.WithDescription));
    }

    [Fact]
    public void DescriptionFor_FallsBackToDisplayAttribute()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("From DisplayAttribute", html.DescriptionFor(m => m.WithDisplayDescription));
    }

    [Fact]
    public void DescriptionFor_PrefersDescriptionAttributeOverDisplayAttribute()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Equal("DescriptionAttribute wins", html.DescriptionFor(m => m.WithBoth));
    }

    [Fact]
    public void DescriptionFor_WithoutEitherAttribute_ReturnsNull()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Null(html.DescriptionFor(m => m.Plain));
    }

    [Fact]
    public void DescriptionFor_WorksOnPagedListModel()
    {
        var html = CreateHelper<PagedList<DescribedModel>>();

        Assert.Equal("From DescriptionAttribute", html.DescriptionFor(m => m.WithDescription));
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

    // On ASP.NET Core, DisplayNameFor is a null-guard plus a one-line delegation to the
    // framework's DisplayNameForInnerType, which resolves display text through view
    // infrastructure a mocked IHtmlHelper cannot supply (it returns null here). Only the
    // guards are unit-testable; covering the resolved text needs an end-to-end test
    // rendering a real view, e.g. against test/site/WebApplicationMvc.
    //
    // The .NET Framework branch does compute the name itself
    // (metadata.DisplayName ?? metadata.PropertyName ?? ...) and is worth covering in
    // Gobln.Pager.Mvc48Test, which now actually runs.

    [Fact]
    public void DisplayNameFor_NullExpression_Throws()
    {
        var html = CreateHelper<Page<DescribedModel>>();

        Assert.Throws<ArgumentNullException>(
            () => html.DisplayNameFor<DescribedModel, string?>(null!));
    }

    #endregion DisplayNameFor
}

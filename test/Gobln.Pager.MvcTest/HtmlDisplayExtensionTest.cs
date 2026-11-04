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

    /// <summary>
    /// The framework's own HtmlHelper, given a view context. DisplayNameFor delegates to its
    /// DisplayNameForInnerType, which a mocked IHtmlHelper can't answer.
    /// </summary>
    private static IHtmlHelper<TModel> CreateViewHelper<TModel>()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddMvcCore()
            .AddViews()
            .AddDataAnnotations()
            .Services
            .BuildServiceProvider();

        var helper = services.GetRequiredService<IHtmlHelper<TModel>>();
        var metadataProvider = services.GetRequiredService<IModelMetadataProvider>();

        var viewContext = new ViewContext(
            HelperHtml.CreateMockActionContext(services: services),
            new Mock<Microsoft.AspNetCore.Mvc.ViewEngines.IView>().Object,
            new ViewDataDictionary<TModel>(metadataProvider, new ModelStateDictionary()),
            new Mock<ITempDataDictionary>().Object,
            TextWriter.Null,
            new HtmlHelperOptions());

        ((IViewContextAware)helper).Contextualize(viewContext);

        return helper;
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

    // On ASP.NET Core, DisplayNameFor delegates to the framework's DisplayNameForInnerType,
    // which resolves the name through view infrastructure: these use the framework's own
    // HtmlHelper (CreateViewHelper). The .NET Framework branch computes the name itself and
    // is covered in Gobln.Pager.Mvc48Test.

    [Fact]
    public void DisplayNameFor_ReadsTheDisplayName()
    {
        var html = CreateViewHelper<Page<DescribedModel>>();

        Assert.Equal("Friendly name", html.DisplayNameFor(m => m.WithDisplayName));
    }

    [Fact]
    public void DisplayNameFor_WithoutADisplayName_IsThePropertyName()
    {
        var html = CreateViewHelper<Page<DescribedModel>>();

        Assert.Equal("Plain", html.DisplayNameFor(m => m.Plain));
    }

    [Fact]
    public void DisplayNameFor_WorksOnPagedListModel()
    {
        var html = CreateViewHelper<PagedList<DescribedModel>>();

        Assert.Equal("Friendly name", html.DisplayNameFor(m => m.WithDisplayName));
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

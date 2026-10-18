using Gobln.Pager;
using Gobln.Pager.Blazor;
using Gobln.Pager.BlazorTest.Helpers;
using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Gobln.Pager.BlazorTest;

public class PagerComponentTests01
{
    private static void InvokeOnParametersSet(ComponentBase component)
    {
        var mi = component.GetType().GetMethod("OnParametersSet", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (mi == null) throw new InvalidOperationException("OnParametersSet method not found.");

        try
        {
            mi.Invoke(component, null);
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            // rethrow inner so xUnit can assert on the original exception type
            throw ex.InnerException;
        }
    }

    [Fact]
    public void OnParametersSet_WithNullPage_ThrowsArgumentNullException()
    {
        var pager = new Blazor.Pager();

        // Page is not set (null) -> OnParametersSet should throw ArgumentNullException
        Assert.Throws<ArgumentNullException>(() => InvokeOnParametersSet(pager));
    }

    [Fact]
    public void PageIndex_OfFirstNonEmptyPage_IsAdopted()
    {
        // A pager first rendered while its data is still loading (an empty Page) must
        // still adopt the index of the real Page once it arrives -- including when the
        // loading placeholder already carried that same index.
        var pager = new Blazor.Pager
        {
            Page = new TestPage { PageIndex = 3, TotalPageCount = 0, HasPaging = false }
        };

        InvokeOnParametersSet(pager);

        pager.Page = new TestPage { PageIndex = 3, TotalPageCount = 10, HasPaging = true };

        InvokeOnParametersSet(pager);

        var pageIndexField = pager.GetType().GetField("PageIndex", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(pageIndexField);

        Assert.Equal(3, (int)pageIndexField.GetValue(pager)!);
    }
}
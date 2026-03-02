using Bunit;
using Gobln.Pager;
using Gobln.Pager.Blazor;
using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Gobln.Pager.BlazorTest;

// Minimal IPage implementation used for tests
internal class TestPage : IPage
{
    public int PageIndex { get; set; }

    public int TotalPageCount { get; set; }

    public int PageSize => throw new NotImplementedException();

    public int TotalItemCount => throw new NotImplementedException();

    public bool HasPaging { get; set; }

    public bool IsEmpty { get; set; }
}

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
    public void StartAndEndNumbers_AreCalculatedCorrectly()
    {
        var pager = new Blazor.Pager
        {
            Page = new TestPage { PageIndex = 5, TotalPageCount = 10, HasPaging = true },
            SiblingCount = 1,
            BoundaryCount = 1,
            ShowEllipsis = false
        };

        // Initialize internal state
        InvokeOnParametersSet(pager);

        // Access private properties via reflection
        var startProp = pager.GetType().GetProperty("StartNumber", BindingFlags.Instance | BindingFlags.NonPublic);
        var endProp = pager.GetType().GetProperty("EndNumber", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(startProp);
        Assert.NotNull(endProp);

        var startNumber = (int)startProp.GetValue(pager)!;
        var endNumber = (int)endProp.GetValue(pager)!;

        // Expected values computed from the component logic:
        // For PageIndex=5, SiblingCount=1, BoundaryCount=1, TotalPageCount=10
        Assert.Equal(4, startNumber);
        Assert.Equal(6, endNumber);
    }
}
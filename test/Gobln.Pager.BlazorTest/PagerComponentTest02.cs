using Bunit;
using Gobln.Pager;
using Gobln.Pager.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Gobln.Pager.BlazorTest;

public class PagerComponentTests02 : BunitContext
{
    [Fact]
    public async Task InvokePageChange_UpdatesPageIndexAndInvokesCallback()
    {
        //todo use bunit

        //https://blog.logrocket.com/testing-in-blazor-a-tutorial/
        //https://stackoverflow.com/questions/77901340/test-that-a-blazor-eventcallback-is-called

        var testPage = new TestPage { PageIndex = 3, TotalPageCount = 10, HasPaging = true };

        using var ctx = new BunitContext();

        var pageChanged = 0;

        Action<int> onPageChanged = c => { pageChanged = c; };

        var cut = Render<Blazor.Pager>(
          parameters =>
            parameters
              .Add(c => c.Page, testPage)
              .Add(c => c.PageIndexChanged, onPageChanged)
        );

        var pager = new Blazor.Pager
        {
            Page = new TestPage { PageIndex = 3, TotalPageCount = 10, HasPaging = true }
        };

        var buttonElement = cut.Find("button");

        buttonElement.Click();

        //Assert.Equal(5, currentPageIndex);
        //Assert.Equal(5, invokedPage);
    }
}
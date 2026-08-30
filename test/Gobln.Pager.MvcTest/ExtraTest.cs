using Gobln.Pager.Mvc;
using Xunit;

namespace Gobln.Pager.MvcTest;

public  class ExtraTest
{
    [Fact]
    public void CloneTest01()
    {
        var target = new PagerOptions()
        {
            NavTagHtmlAttributes = new { aria_label = "Page navigation example" },
            UlTagHtmlAttributes = new { @class = "justify-content-end" },

            AllowDisabledPageLink = false,
            AllowDisabledPageLinkTab = false,

            DisplayActive = false,

            HRefUrl = "#",

            ShowButtonFirstLink = WhenToShowEnum.Never,
            ShowButtonLastLink = WhenToShowEnum.Never,
        };

        var result = target.Clone();

        Assert.Equivalent(target, result, strict:true);
    }
}

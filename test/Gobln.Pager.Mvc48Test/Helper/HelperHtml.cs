using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gobln.Pager.Mvc48Test.Helper;

public class HelperHtml
{
    public HtmlHelper CreateHtmlHelper(ViewDataDictionary? viewData = null, string? url = null)
    {
        return CreateHtmlHelper<string>(viewData, url);
    }

    public HtmlHelper<T> CreateHtmlHelper<T>(ViewDataDictionary? viewData = null, string? url = null)
    {
        viewData ??= new ViewDataDictionary("Test");

        var request = new Mock<HttpRequestBase>();

        if (url is not null)
        {
            request.Setup(r => r.Url).Returns(new Uri(url));
        }

        var hcb = new Mock<HttpContextBase>();
        hcb.Setup(c => c.Request).Returns(request.Object);

        var cc = new Mock<ControllerContext>(
            hcb.Object,
            //hcb,
            new RouteData(),
            new Mock<ControllerBase>().Object);

        cc.Setup(c => c.HttpContext).Returns(hcb.Object);

        var mockViewContext = new Mock<ViewContext>(
            cc.Object,
            new Mock<IView>().Object,
            viewData,
            new TempDataDictionary(),
            TextWriter.Null);

        mockViewContext.Setup(c => c.HttpContext).Returns(hcb.Object);

        var mockViewDataContainer = new Mock<IViewDataContainer>();

        mockViewDataContainer.Setup(v => v.ViewData).Returns(viewData);

        return new HtmlHelper<T>(mockViewContext.Object, mockViewDataContainer.Object);
    }
}

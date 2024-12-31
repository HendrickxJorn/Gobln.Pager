using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Gobln.Pager.MvcTest.Helper
{
    public class HelperHtml
    {
        public IHtmlHelper CreateHtmlHelper(string? url = null)
        {
            var mockHtmlHelper = new Mock<IHtmlHelper>();

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();
            var mockViewContext = new Mock<ViewContext>();

            if (url is not null)
            {
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

                var vc = new ViewContext(CreateMockActionContext(url), 
                    new Mock<Microsoft.AspNetCore.Mvc.ViewEngines.IView>().Object,
                    viewData,
                    new Mock<ITempDataDictionary>().Object,
                    new Mock<TextWriter>().Object,
                    new Mock<HtmlHelperOptions>().Object);

                mockHtmlHelper.Setup(h => h.ViewContext).Returns(vc);
            }

            return mockHtmlHelper.Object;
        }

        public static ActionContext CreateMockActionContext(string? url = null)
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();
            var mockHttpResponse = new Mock<HttpResponse>();
            var mockRouteData = new Mock<RouteData>();
            var mockActionDescriptor = new Mock<ActionDescriptor>();

            if (url != null)
            {
                var uri = new Uri(url);
                mockHttpRequest.Setup(r => r.Scheme).Returns(uri.Scheme);
                mockHttpRequest.Setup(r => r.Host).Returns(new HostString(uri.Host));
                mockHttpRequest.Setup(r => r.Path).Returns(new PathString(uri.AbsolutePath));
                mockHttpRequest.Setup(r => r.QueryString).Returns(new QueryString(uri.Query));
            }

            mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockHttpResponse.Object);

            return new ActionContext(
                mockHttpContext.Object,
                mockRouteData.Object,
                mockActionDescriptor.Object,
                new ModelStateDictionary()
            );
        }
    }
}

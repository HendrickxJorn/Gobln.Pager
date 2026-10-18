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
        /// <param name="url">The url of the request, as the browser sent it. Null: no request at all.</param>
        /// <param name="pathBase">The path the app is mounted on. It is then left out of the request's Path.</param>
        /// <param name="hasHost">False for a request without a Host header.</param>
        /// <param name="services">The app's services, as HttpContext.RequestServices. Only with a <paramref name="url"/>.</param>
        public IHtmlHelper CreateHtmlHelper(string? url = null, string? pathBase = null, bool hasHost = true, IServiceProvider? services = null)
        {
            var mockHtmlHelper = new Mock<IHtmlHelper>();

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();
            var mockViewContext = new Mock<ViewContext>();

            if (url is not null)
            {
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

                var vc = new ViewContext(CreateMockActionContext(url, pathBase, hasHost, services),
                    new Mock<Microsoft.AspNetCore.Mvc.ViewEngines.IView>().Object,
                    viewData,
                    new Mock<ITempDataDictionary>().Object,
                    new Mock<TextWriter>().Object,
                    new Mock<HtmlHelperOptions>().Object);

                mockHtmlHelper.Setup(h => h.ViewContext).Returns(vc);
            }

            return mockHtmlHelper.Object;
        }

        public static ActionContext CreateMockActionContext(string? url = null, string? pathBase = null, bool hasHost = true, IServiceProvider? services = null)
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
                mockHttpRequest.Setup(r => r.Host).Returns(hasHost ? new HostString(uri.Host) : default);
                mockHttpRequest.Setup(r => r.PathBase).Returns(new PathString(pathBase));

                // Like the server does, the app only gets the part of the path after its path base
                var path = pathBase is null ? uri.AbsolutePath : uri.AbsolutePath.Substring(pathBase.Length);

                mockHttpRequest.Setup(r => r.Path).Returns(new PathString(path));
                mockHttpRequest.Setup(r => r.QueryString).Returns(new QueryString(uri.Query));
            }

            mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(c => c.Response).Returns(mockHttpResponse.Object);
            mockHttpContext.Setup(c => c.RequestServices).Returns(services!);

            return new ActionContext(
                mockHttpContext.Object,
                mockRouteData.Object,
                mockActionDescriptor.Object,
                new ModelStateDictionary()
            );
        }
    }
}

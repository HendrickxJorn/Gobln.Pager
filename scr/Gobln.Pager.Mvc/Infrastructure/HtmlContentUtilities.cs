//using Microsoft.AspNetCore.Html;
//using Microsoft.Extensions.WebEncoders.Testing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;

//namespace Gobln.Pager.Mvc.Infrastructure
//{
//    internal class HtmlContentUtilities
//    {
//        public static string HtmlContentToString(IHtmlContent content, HtmlEncoder encoder = null)
//        {
//            if (encoder == null)
//            {
//                encoder = new HtmlTestEncoder();
//            }

//            using (var writer = new StringWriter())
//            {
//                content.WriteTo(writer, encoder);
//                return writer.ToString();
//            }
//        }
//    }
//}

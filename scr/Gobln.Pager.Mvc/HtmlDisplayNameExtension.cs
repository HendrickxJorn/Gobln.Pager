using System.Linq.Expressions;

#if NETFRAMEWORK

using System.Web;
using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Mvc.Rendering;

#endif

namespace Gobln.Pager.Mvc;

/// <summary>
/// Provides a mechanism to get display names for Page
/// </summary>
public static class HtmlDisplayNameExtension
{
    /// <summary>
    /// Gets the display name for the model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="expression">An expression that identifies the object that contains the display name.</param>
    /// <returns>The display name for the model.</returns>
#if NETFRAMEWORK
    public static MvcHtmlString DisplayNameFor<TModel, TValue>(this HtmlHelper<Page<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression)
#else
    public static string DisplayNameFor<TModel, TValue>(this IHtmlHelper<Page<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression)
#endif
    {
        if (htmlHelper == null)
        {
            throw new ArgumentNullException(nameof(htmlHelper));
        }

        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

#if NETFRAMEWORK
        var metadata = ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<TModel>());

        return new MvcHtmlString(HttpUtility.HtmlEncode(metadata.DisplayName ?? metadata.PropertyName ?? ExpressionHelper.GetExpressionText(expression).Split('.').Last()));
#else
        return htmlHelper.DisplayNameForInnerType(expression);
#endif
    }

    /// <summary>
    /// Gets the display name for the model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="expression">An expression that identifies the object that contains the display name.</param>
    /// <returns>The display name for the model.</returns>
#if NETFRAMEWORK
public static MvcHtmlString DisplayNameFor<TModel, TValue>(this HtmlHelper<PagedList<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression)
#else
    public static string DisplayNameFor<TModel, TValue>(this IHtmlHelper<PagedList<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression)
#endif
    {
        if (htmlHelper == null)
        {
            throw new ArgumentNullException(nameof(htmlHelper));
        }

        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

#if NETFRAMEWORK
        var metadata = ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<TModel>());

        return new MvcHtmlString(HttpUtility.HtmlEncode(metadata.DisplayName ?? metadata.PropertyName ?? ExpressionHelper.GetExpressionText(expression).Split('.').Last()));
#else
        return htmlHelper.DisplayNameForInnerType(expression);
#endif
    }
}

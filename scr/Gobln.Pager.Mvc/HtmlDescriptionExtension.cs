using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq.Expressions;

#if NETFRAMEWORK

using System.Web;
using System.Web.Mvc;

#else

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

#endif


namespace Gobln.Pager.Mvc;

/// <summary>
/// Provides a mechanism to get display names for Page
/// </summary>
public static class HtmlDescriptionExtension
{
    /// <summary>
    /// Gets the description name for the model.
    /// If DescriptionAttribute is not found than will look for DisplayAttribute to get the description from.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="expression">An expression that identifies the object that contains the description name.</param>
    /// <returns>The description name for the model.</returns>
#if NETFRAMEWORK
    public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<Page<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression)
#else
    public static string? DescriptionFor<TModel, TValue>(this IHtmlHelper<Page<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression)
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

        var m = ((MemberExpression)expression.Body).Member;

#if NETFRAMEWORK
        return new MvcHtmlString(HttpUtility.HtmlEncode(((DescriptionAttribute)Attribute.GetCustomAttribute(m, typeof(DescriptionAttribute), false))?.Description
                    ?? ((DisplayAttribute)Attribute.GetCustomAttribute(m, typeof(DisplayAttribute), false))?.Description));
#else
        return m.GetCustomAttribute<DescriptionAttribute>(false)?.Description ?? m.GetCustomAttribute<DisplayAttribute>(false)?.Description;
#endif
    }

    /// <summary>
    /// Gets the description name for the model.
    /// If DescriptionAttribute is not found than will look for DisplayAttribute to get the description from.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="expression">An expression that identifies the object that contains the description name.</param>
    /// <returns>The description name for the model.</returns>
#if NETFRAMEWORK
    public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<PagedList<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression)
#else
    public static string? DescriptionFor<TModel, TValue>(this IHtmlHelper<PagedList<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression)
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

        var m = ((MemberExpression)expression.Body).Member;

#if NETFRAMEWORK
        return new MvcHtmlString(HttpUtility.HtmlEncode(((DescriptionAttribute)Attribute.GetCustomAttribute(m, typeof(DescriptionAttribute), false))?.Description
                    ?? ((DisplayAttribute)Attribute.GetCustomAttribute(m, typeof(DisplayAttribute), false))?.Description));
#else
        return m.GetCustomAttribute<DescriptionAttribute>(false)?.Description ?? m.GetCustomAttribute<DisplayAttribute>(false)?.Description;
#endif
    }
}

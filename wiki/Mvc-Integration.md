# MVC / ASP.NET Core Integration (`Gobln.Pager.Mvc`)

Targets `net462`, `net48`, `net8.0`, `net10.0`. Namespace `Gobln.Pager.Mvc`. Compiles against classic `System.Web.Mvc` (`MvcHtmlString`) on .NET Framework, and against `Microsoft.AspNetCore.Mvc.Rendering` (`IHtmlContent`) on modern .NET — the public method shapes below are the same either way, just with a different return type.

## Rendering a pager: `Html.Pager(...)`

```csharp
IHtmlContent Pager(this IHtmlHelper html, IPage page);
IHtmlContent Pager(this IHtmlHelper html, IPage page, IPagerOptions pagerOptions);
IHtmlContent Pager(this IHtmlHelper html, IPage page, IPagerRenderer pagerRenderer);
IHtmlContent Pager<T>(this IHtmlHelper html, IPage page, T pagerOptions, IPagerRenderer<T> pagerRenderer) where T : IPagerOptions;
```

Simplest usage, using whatever options were registered via `AddMvcPager()` (or the library defaults if none were registered):

```cshtml
@using Gobln.Pager.Mvc
@model Page<YourEntity>

@Html.Pager(Model)
```

With one-off options:

```cshtml
@Html.Pager(Model, new PagerOptions { SiblingCount = 2, BoundaryCount = 2 })
```

When no explicit `HRefUrl` is set on the options, the helper resolves the url from the current request (via an internal `UrlHelper`), so page links point back to the current page/action with an updated page-index query parameter. The rest of the query string is kept, so filters and sorting survive paging, and so is the path base of an app hosted under a virtual directory. The links are relative (path and query, no scheme or host): a request's host comes from its `Host` header, which the client controls, so it never ends up in the page. The resolved url is passed to the renderer separately and never written back into the options, so one options instance can be shared between views and requests.

## `PagerOptions`

Implements `IPagerOptions`; construct directly or via `Clone()`. Defaults shown:

| Property | Default | Description |
| --- | --- | --- |
| `ShowPager` | `IfNecessary` | Whether to render the pager at all (`Never` / `IfNecessary` / `Always`). |
| `ShowButtonFirstLink` | `Always` | Show the "First" link. |
| `ShowButtonPreviousLink` | `Always` | Show the "Previous" link. |
| `ShowPagerButtonLinks` | `Always` | Show numbered page links. `IfNecessary` shows them only when there is more than one page. |
| `ShowButtonNextLink` | `Always` | Show the "Next" link. |
| `ShowButtonLastLink` | `Always` | Show the "Last" link. |
| `SiblingCount` | `1` | Page links shown on each side of the current page. |
| `BoundaryCount` | `1` | Page links always shown at the start/end. |
| `ShowEllipsis` | `true` | Render a disabled `...` item where page numbers are skipped. An ellipsis that would hide a single page shows that page instead, the same as the Blazor pager. |
| `ShowIndexAsRange` | `false` | Show the item range (`11 - 20`) instead of the page index. The last range ends at the last item. |
| `ButtonFirstLinkLabel` / `ButtonPreviousLinkLabel` / `ButtonNextLinkLabel` / `ButtonLastLinkLabel` | `"First"` / `"Previous"` / `"Next"` / `"Last"` | Link text. Escaped on every target. |
| `ButtonFirstLinkIcon` / `ButtonPreviousLinkIcon` / `ButtonNextLinkIcon` / `ButtonLastLinkIcon` | `null` | Icon markup rendered in place of the link text, not escaped (for example `&laquo;` or `<i class="bi bi-chevron-left"></i>`); the label then becomes the `aria-label`. |
| `HRefUrl` | `null` | Explicit URL for page links. `null` = the current request's path and query, as a relative link; `"#"` = all links point to `#`. |
| `PageIndexUrlName` | `"pageIndex"` | Query-string key used for the page index in generated links. |
| `PageIndexDataName` | `null` | When set, links also get a `data-{name}="{page index}"` attribute, for pagers driven by JavaScript. |
| `DisplayActive` | `true` | Mark the current page number: `active` on its `<li>` and `aria-current="page"` on its link. |
| `AllowDisabledPageLink` | `true` | Disable the buttons that point at the current page, and the current page number when `DisplayActive` is `false`: `disabled` class, rendered as a `<span>`. When `false` they stay working links. The active page itself is always a link with `aria-current="page"`, as in Bootstrap 5.3. |
| `NavTagHtmlAttributes` / `UlTagHtmlAttributes` | `null` | Attributes for the `<nav>` / `<ul>`. Without an `aria-label` or `aria-labelledby`, the `<nav>` gets `aria-label="Page navigation"`. |

`WhenToShowEnum` values: `Never = 0`, `IfNecessary = 1`, `Always = 2`.

## Registering default options

ASP.NET Core (`net8.0`+):

```csharp
// Program.cs
builder.Services.AddMvcPager();
// or
builder.Services.AddMvcPager(new PagerOptions { SiblingCount = 2 });
// or
builder.Services.AddMvcPager(options => options.BoundaryCount = 2);
```

These set the defaults used whenever `Html.Pager(page)` is called without explicit options. `AddMvcPager(options)` accepts any `IPagerOptions` and stores a copy. The defaults are registered as options in the app's services (`IOptions<PagerOptions>`), not in a process-wide static, so two apps in one process (two test hosts, for example) each keep their own.

`AddMvcPager` is only compiled for the modern .NET targets. On .NET Framework, set the public `PagerSettings.DefaultPagerOptions` instead, for example in `Application_Start`:

```csharp
PagerSettings.DefaultPagerOptions = new PagerOptions { SiblingCount = 2 };
```

## Custom rendering via `IPagerRenderer`

`Html.Pager(page, pagerOptions, pagerRenderer)` lets you swap in your own `IPagerRenderer<T>` implementation. The built-in `PagerRenderer : IPagerRenderer<IPagerOptions>` builds the `<nav><ul class="pagination">…</ul></nav>` markup via a set of `virtual TagCreator*` methods, so you can subclass it to change individual pieces of the markup instead of writing a renderer from scratch.

`IPagerRenderer<T>.Render(IPage page, T options, string hRefUrl)` receives the page, the options and the url already resolved for the current request. Inside `PagerRenderer`'s overrides, use `HRefUrl` (or `UrlCreator(pageIndex)`) rather than `PagerOptions.HRefUrl`, which may still be `null`. A `PagerRenderer` keeps nothing of a render on itself, so one instance can be shared between views and requests.

`Html.Pager(page, pagerRenderer)` is for an `IPagerRenderer` that needs nothing but the page: its `Render(IPage page)` gets no options and no url. `PagerRenderer` is not one, so pass it together with options.

## Display helpers

Two additional helpers work against `IHtmlHelper<Page<TModel>>` / `IHtmlHelper<PagedList<TModel>>` for label text, typically used in a strongly-typed view whose model is a `Page<T>` or `PagedList<T>`:

```csharp
string? Html.DescriptionFor(Expression<Func<TModel, TValue>> expression);
string Html.DisplayNameFor(Expression<Func<TModel, TValue>> expression);
```

- `DescriptionFor` reads a `[Description]` attribute on the target member, falling back to `[Display].Description`.
- `DisplayNameFor` delegates to the standard ASP.NET Core display-name resolution (`DisplayNameForInnerType`) — equivalent to `Html.DisplayNameFor` on the underlying item type.
- On .NET Framework both return an HTML-encoded `MvcHtmlString` instead of a `string`.

```cshtml
@model PagedList<YourEntity>

<th>@Html.DisplayNameFor(m => m.Name)</th>
<td title="@Html.DescriptionFor(m => m.Name)">...</td>
```

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
@Html.Pager(Model, new PagerOptions { SiblingCount = 2, ShowEllipsis = true })
```

When no explicit `HRefUrl` is set on the options, the helper automatically fills it in from the current request URL (via an internal `UrlHelper`), so page links point back to the current page/action with an updated page-index query parameter.

## `PagerOptions`

Implements `IPagerOptions`; construct directly or via `Clone()`. Defaults shown:

| Property | Default | Description |
|---|---|---|
| `ShowPager` | `IfNecessary` | Whether to render the pager at all (`Never` / `IfNecessary` / `Always`). |
| `ShowButtonFirstLink` | `Always` | Show the "First" link. |
| `ShowButtonPreviousLink` | `Always` | Show the "Previous" link. |
| `ShowPagerButtonLinks` | `Always` | Show numbered page links. |
| `ShowButtonNextLink` | `Always` | Show the "Next" link. |
| `ShowButtonLastLink` | `Always` | Show the "Last" link. |
| `SiblingCount` | `1` | Page links shown on each side of the current page. |
| `BoundaryCount` | `1` | Page links always shown at the start/end. |
| `ShowEllipsis` | `true` | Render `…` between boundary and sibling ranges. |
| `ShowIndexAsRange` | `false` | Show an item range instead of a page index (label text only; see `todo` comments in source — not yet wired up to output text). |
| `ButtonFirstLinkLabel` / `ButtonPreviousLinkLabel` / `ButtonNextLinkLabel` / `ButtonLastLinkLabel` | `"First"` / `"Previous"` / `"Next"` / `"Last"` | Link text. |
| `ButtonFirstLinkIcon` / `ButtonPreviousLinkIcon` / `ButtonNextLinkIcon` / `ButtonLastLinkIcon` | `null` | Icon markup rendered in place of the link text; the label then becomes the `aria-label`. |
| `HRefUrl` | `null` | Explicit URL for page links. `null` = current request URL; `"#"` = all links point to `#`. |
| `PageIndexUrlName` | `"pageIndex"` | Query-string key used for the page index in generated links. |
| `AllowDisabledPageLink` / `AllowDisabledPageLinkTab` | `true` | Whether disabled links are still rendered/tabbable. |

`WhenToShowEnum` values: `Never = 0`, `IfNecessary = 1`, `Always = 2`.

## Registering default options (ASP.NET Core, `net8.0`+ only)

```csharp
// Program.cs
builder.Services.AddMvcPager();
// or
builder.Services.AddMvcPager(new PagerOptions { SiblingCount = 2 });
// or
builder.Services.AddMvcPager(options => options.ShowEllipsis = true);
```

These set `PagerSettings.DefaultPagerOptions`, used whenever `Html.Pager(page)` is called without explicit options. `AddMvcPager` is only compiled for the modern .NET targets — on .NET Framework targets, construct and pass a `PagerOptions` instance explicitly instead.

## Custom rendering via `IPagerRenderer`

`Html.Pager(page, pagerRenderer)` / `Html.Pager(page, pagerOptions, pagerRenderer)` let you swap in your own `IPagerRenderer` / `IPagerRenderer<T>` implementation. The built-in `PagerRenderer : IPagerRenderer<IPagerOptions>` builds the `<nav><ul class="pagination">…</ul></nav>` markup via a set of `virtual TagCreator*` methods, so you can subclass it to change individual pieces of the markup instead of writing a renderer from scratch.

## Display helpers

Two additional helpers work against `IHtmlHelper<Page<TModel>>` / `IHtmlHelper<PagedList<TModel>>` for label text, typically used in a strongly-typed view whose model is a `Page<T>` or `PagedList<T>`:

```csharp
string? Html.DescriptionFor(Expression<Func<TModel, TValue>> expression);
string Html.DisplayNameFor(Expression<Func<TModel, TValue>> expression);
```

- `DescriptionFor` reads a `[Description]` attribute on the target member, falling back to `[Display].Description`.
- `DisplayNameFor` delegates to the standard ASP.NET Core display-name resolution (`DisplayNameForInnerType`) — equivalent to `Html.DisplayNameFor` on the underlying item type.

```cshtml
@model PagedList<YourEntity>

<th>@Html.DisplayNameFor(m => m.Name)</th>
<td title="@Html.DescriptionFor(m => m.Name)">...</td>
```

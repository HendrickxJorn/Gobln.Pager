[![NuGet](https://img.shields.io/nuget/dt/Gobln.Pager.mvc.svg)](http://www.nuget.org/packages/Gobln.Pager.Mvc/)
[![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.mvc.svg)](https://www.nuget.org/packages/Gobln.Pager.Mvc/)

# Gobln.Pager.Mvc

HTML helpers that render a [Gobln.Pager](https://www.nuget.org/packages/Gobln.Pager/) `Page` or `PagedList` as pagination markup in a Razor view. The markup follows Bootstrap's pagination component, so it works with Bootstrap 5.3 without any extra CSS. The library ships no CSS of its own.

## Frameworks

- ASP.NET Core: .NET 8.0 and .NET 10.0
- ASP.NET MVC 5: .NET Framework 4.6.2 and 4.8

## Installation

```shell
dotnet add package Gobln.Pager.Mvc
```

## Quick start

Return a page from the controller. Name the parameter `pageIndex`: that is the query-string key the pager's links use.

```csharp
using Gobln.Pager;

public IActionResult Index(int pageIndex = 1)
{
    var page = _db.Orders.OrderBy(o => o.Id).ToPage(pageIndex, 10);

    return View(page);
}
```

Render the items and the pager in the view:

```cshtml
@using Gobln.Pager
@using Gobln.Pager.Mvc
@model Page<Order>

@foreach (var order in Model)
{
    <p>@order.Number</p>
}

@Html.Pager(Model)
```

On page 1 of 10, at `https://example.com/orders`, this renders:

```html
<nav aria-label="Page navigation">
  <ul class="pagination">
    <li class="disabled page-item"><span class="page-link">First</span></li>
    <li class="disabled page-item"><span class="page-link">Previous</span></li>
    <li class="active page-item"><a aria-current="page" class="page-link" href="https://example.com/orders?pageIndex=1">1</a></li>
    <li class="page-item"><a class="page-link" href="https://example.com/orders?pageIndex=2">2</a></li>
    <li class="page-item"><a class="page-link" href="https://example.com/orders?pageIndex=3">3</a></li>
    <li class="page-item"><a class="page-link" href="https://example.com/orders?pageIndex=4">4</a></li>
    <li class="page-item"><a class="page-link" href="https://example.com/orders?pageIndex=5">5</a></li>
    <li class="disabled page-item"><span class="page-link">...</span></li>
    <li class="page-item"><a class="page-link" href="https://example.com/orders?pageIndex=10">10</a></li>
    <li class="page-item"><a class="page-link" href="https://example.com/orders?pageIndex=2">Next</a></li>
    <li class="page-item"><a class="page-link" href="https://example.com/orders?pageIndex=10">Last</a></li>
  </ul>
</nav>
```

Links keep the rest of the current query string, so filters and sorting survive paging. When everything fits on one page, the pager renders nothing.

## Options

Pass options for one pager:

```cshtml
@Html.Pager(Model, new PagerOptions { SiblingCount = 2, BoundaryCount = 2 })
```

Or set the defaults for every `@Html.Pager(Model)` once, at startup.

ASP.NET Core, in `Program.cs`:

```csharp
builder.Services.AddMvcPager(options =>
{
    options.ButtonPreviousLinkLabel = "Vorige";
    options.ButtonNextLinkLabel = "Volgende";
});
```

`AddMvcPager(new PagerOptions { ... })` works too. It copies the options, so later changes to that object don't affect the defaults.

ASP.NET MVC 5, in `Application_Start`:

```csharp
PagerSettings.DefaultPagerOptions = new PagerOptions { SiblingCount = 2 };
```

An options object can be shared between views and requests (for example a `static readonly` field): the pager only reads it.

| Option | Default | What it does |
| --- | --- | --- |
| `ShowPager` | `IfNecessary` | Render the pager at all. `IfNecessary`: only when there is more than one page. |
| `ShowButtonFirstLink` | `Always` | The First button. `IfNecessary`: only when not on the first page. |
| `ShowButtonPreviousLink` | `Always` | The Previous button. `IfNecessary`: only when not on the first page. |
| `ShowPagerButtonLinks` | `Always` | The numbered page links. `IfNecessary`: only when there is more than one page. |
| `ShowButtonNextLink` | `Always` | The Next button. `IfNecessary`: only when not on the last page. |
| `ShowButtonLastLink` | `Always` | The Last button. `IfNecessary`: only when not on the last page. |
| `SiblingCount` | `1` | Page numbers shown on each side of the current page. |
| `BoundaryCount` | `1` | Page numbers always shown at the start and the end. |
| `ShowEllipsis` | `true` | Show `...` where page numbers are skipped. |
| `ShowIndexAsRange` | `false` | Show the item range (`11 - 20`) instead of the page number. |
| `ButtonFirstLinkLabel`, `ButtonPreviousLinkLabel`, `ButtonNextLinkLabel`, `ButtonLastLinkLabel` | `"First"`, `"Previous"`, `"Next"`, `"Last"` | Button text. Inserted as text, so `&` and `<` are escaped. |
| `ButtonFirstLinkIcon`, `ButtonPreviousLinkIcon`, `ButtonNextLinkIcon`, `ButtonLastLinkIcon` | `null` | Shown instead of the label. Inserted as markup, not escaped. The label becomes the button's `aria-label`. |
| `DisplayActive` | `true` | Mark the current page number: `active` on its `<li>` and `aria-current="page"` on its link. It stays a working link, as in Bootstrap 5.3. |
| `AllowDisabledPageLink` | `true` | Disable the buttons that point at the current page (First and Previous on the first page, for example), and the current page number when `DisplayActive` is `false`. A disabled item gets the `disabled` class and renders as a `<span>`, so it can't be clicked. When `false`, they stay working links. |
| `HRefUrl` | `null` | The url the links are built on. `null`: the current request's url. `"#"`: every link is `#`, for pagers driven by JavaScript. |
| `PageIndexUrlName` | `"pageIndex"` | The query-string key for the page index. |
| `PageIndexDataName` | `null` | When set, links also get a `data-{name}="{page index}"` attribute. |
| `NavTagHtmlAttributes` | `null` | Attributes for the `<nav>`. Without an `aria-label` or `aria-labelledby`, the pager adds `aria-label="Page navigation"`. |
| `UlTagHtmlAttributes` | `null` | Attributes for the `<ul>`. Classes are added next to `pagination`. |

For the attribute options, use an anonymous object. An underscore becomes a hyphen: `new { aria_label = "Search results pages" }` renders `aria-label="Search results pages"`.

## Bootstrap 5.3

Include Bootstrap's CSS in your layout. The examples below are Bootstrap's own pagination examples.

### Sizing

```cshtml
@Html.Pager(Model, new PagerOptions { UlTagHtmlAttributes = new { @class = "pagination-lg" } })
@Html.Pager(Model, new PagerOptions { UlTagHtmlAttributes = new { @class = "pagination-sm" } })
```

### Alignment

```cshtml
@Html.Pager(Model, new PagerOptions { UlTagHtmlAttributes = new { @class = "justify-content-center" } })
@Html.Pager(Model, new PagerOptions { UlTagHtmlAttributes = new { @class = "justify-content-end" } })
```

### Icons

Use an HTML entity:

```cshtml
@Html.Pager(Model, new PagerOptions
{
    ButtonPreviousLinkIcon = "&laquo;",
    ButtonNextLinkIcon = "&raquo;",
    ShowButtonFirstLink = WhenToShowEnum.Never,
    ShowButtonLastLink = WhenToShowEnum.Never,
})
```

The Previous button then renders as:

```html
<li class="page-item">
  <a aria-label="Previous" class="page-link" href="https://example.com/orders?pageIndex=4"><span aria-hidden="true">&laquo;</span></a>
</li>
```

Or [Bootstrap Icons](https://icons.getbootstrap.com/):

```cshtml
@Html.Pager(Model, new PagerOptions
{
    ButtonFirstLinkIcon = "<i class=\"bi bi-chevron-double-left\"></i>",
    ButtonPreviousLinkIcon = "<i class=\"bi bi-chevron-left\"></i>",
    ButtonNextLinkIcon = "<i class=\"bi bi-chevron-right\"></i>",
    ButtonLastLinkIcon = "<i class=\"bi bi-chevron-double-right\"></i>",
})
```

Icons are inserted as markup, so only put markup you wrote yourself in them, never user input.

### Dark mode

Nothing to set: the pager uses Bootstrap's own classes, so it follows `data-bs-theme="dark"`.

## Custom markup

To change part of the markup, subclass `PagerRenderer` and override its `virtual` methods: `TagCreatorNav`, `TagCreatorUl`, `TagCreatorLi`, `TagCreatorLink`, `TagCreatorSpan`, `TagCreatorSpanIcon`, `PageNumbersCreator`, `UrlCreator`, and `CreateFirstTag` / `CreatePreviousTag` / `CreateNextTag` / `CreateLastTag`. Pass an instance with the options:

```cshtml
@Html.Pager(Model, new PagerOptions(), new MyPagerRenderer())
```

Inside the renderer, `PagerOptions` holds the options and `HRefUrl` holds the url resolved for the current request. Build links with `UrlCreator(pageIndex)`: it returns `null` when there is no url to link to, and the link should then fall back to `#`.

To render something that isn't Bootstrap-shaped at all, implement `IPagerRenderer<T>` yourself. `SetOptions(options, hRefUrl)` receives the options and the resolved url before each `Render(page)`.

## Behaviour to know about

- Icons are markup and are never escaped. Labels are text and are always escaped. Both work the same way on ASP.NET Core and ASP.NET MVC 5.
- The pager never writes to the options you pass in. The url of the current request is resolved separately for each render.
- Disabled items are `<span>`s, not links. Bootstrap's `pointer-events: none` only stops the mouse; a screen reader would still announce a disabled link as working.
- The current page is a working link with `aria-current="page"`, following Bootstrap 5.3's pagination example.

## License

[Apache License, Version 2.0](http://opensource.org/licenses/Apache-2.0).

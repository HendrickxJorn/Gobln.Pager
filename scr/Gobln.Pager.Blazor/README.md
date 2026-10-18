[![NuGet](https://img.shields.io/nuget/dt/Gobln.Pager.Blazor.svg)](http://www.nuget.org/packages/Gobln.Pager.Blazor/)
[![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.Blazor.svg)](https://www.nuget.org/packages/Gobln.Pager.Blazor/)

# Gobln.Pager.Blazor

Gobln.Pager.Blazor is a lightweight Blazor pagination component designed for Blazor Server and Blazor WebAssembly apps. It renders a configurable pager UI and emits page-change events.

## Installation

Install the NuGet package:

```
dotnet add package Gobln.Pager.Blazor
```

## Quick Start (Blazor)

1. Add the usings:
``` razor
@using Gobln.Pager
@using Gobln.Pager.Blazor
```
2. Get a page of your data. `ToPage(pageIndex, pageSize)` from Gobln.Pager works on any `IEnumerable<T>` or `IQueryable<T>` and returns a `Page<T>`: the items of that page, plus the paging info the pager needs.
3. Show the items and the `Pager`, and load the new page when `PageIndexChanged` fires:
``` razor
<ul>
    @foreach (var item in currentPage)
    {
        <li>@item</li>
    }
</ul>

<Pager Page="currentPage"
       SiblingCount="1"
       BoundaryCount="1"
       ShowEllipsis="true"
       PageIndexChanged="OnPageIndexChanged" />

@code {
    private const int PageSize = 10;

    private readonly IEnumerable<int> items = Enumerable.Range(1, 200);

    private Page<int> currentPage = default!;

    protected override void OnInitialized() => LoadPage(1);

    private void OnPageIndexChanged(int newPageIndex) => LoadPage(newPageIndex);

    private void LoadPage(int pageIndex) => currentPage = items.ToPage(pageIndex, PageSize);
}
```

## Parameters

- `Page` (required): The `IPage` to show, for example the `Page<T>` returned by `ToPage`. The pager reads its `PageIndex` and `TotalPageCount`. Marked with `[EditorRequired]`.
- `SiblingCount` (int): Number of sibling pages to show on each side of the current page. Default: `1`.
- `BoundaryCount` (int): Number of always visible pages at the beginning and end. Default: `0`.
- `ShowEllipsis` (bool): Whether to show an ellipsis (...) where page numbers are skipped. An ellipsis that would hide a single page shows that page instead. Default: `false`.
- `ClassName` (string): Additional CSS classes appended to the pagination `<ul>`. Default: `null`.
- `PageIndexChanged` (EventCallback<int>): Raised when the user selects another page; provides the new page index.

## Styling

The component renders a `<nav>` containing a `<ul class="pagination">`. It uses a `PagerItem` subcomponent for each item. Style it via your site CSS or a framework (Bootstrap classes are used by default in markup).

The items follow Bootstrap 5.3's pagination markup. The current page is a link with `aria-current="page"`. Disabled items (First and Previous on the first page, Next and Last on the last page, and the ellipsis) are `<span>`s that don't react to clicks.

Use `ClassName` to add your own classes alongside the built-in `pagination` class, for example Bootstrap sizing or alignment modifiers:

```
<Pager Page="@MyPage" ClassName="pagination-lg justify-content-center" />
```

renders:

``` html
<nav aria-label="page navigation">
    <ul class="pagination pagination-lg justify-content-center">
        ...
    </ul>
</nav>
```

## Example: Blazor Server

Add the component to any Razor component or page as shown above. Handle `PageIndexChanged` to load the new page. The pager reacts to clicks, so the page needs an interactive render mode, for example `@rendermode InteractiveServer`.

## Notes

- The package targets modern .NET and is suitable for .NET 8+ Blazor apps.
- The pager only reads `PageIndex` and `TotalPageCount`. If your app has its own paging model, wrap it in a class that implements `IPage` from Gobln.Pager.

## Contributing

Contributions are welcome. Please follow the repository's contribution guidelines and coding conventions.

## License

[MIT](https://github.com/HendrickxJorn/Gobln.Pager/blob/master/LICENSE.md)
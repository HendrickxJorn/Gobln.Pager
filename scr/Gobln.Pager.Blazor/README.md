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

1. Add using (optional):
``` csharp
@using Gobln.Pager.Blazor
```
2. Provide a page model that exposes the required paging properties (the library expects
``` csharp
public class SimplePage : IPage { public int PageIndex { get; set; } public int TotalPageCount { get; set; } }
```
3. Use the `Pager` component in your page or component:
```
<Pager Page="@MyPage" SiblingCount="1" BoundaryCount="1" ShowEllipsis="true" PageChanging="OnPageChanging" />
@code { private SimplePage MyPage = new() { PageIndex = 1, TotalPageCount = 20 };

private async Task OnPageChanging(int newPage)
{
    // update the current page and reload data as required
    MyPage.PageIndex = newPage;
    await LoadDataForPage(newPage);
}

private Task LoadDataForPage(int page)
{
    // fetch items for the requested page
    return Task.CompletedTask;
}
}
```

## Parameters

- `Page` (required): An object implementing the paging contract (needs `PageIndex` and `TotalPageCount`). Marked with `[EditorRequired]`.
- `SiblingCount` (int): Number of sibling pages to show on each side of the current page. Default: `1`.
- `BoundaryCount` (int): Number of always visible pages at the beginning and end. Default: `0`.
- `ShowEllipsis` (bool): Whether to show ellipsis (...) between ranges. Default: `false`.
- `ClassName` (string): Additional CSS classes appended to the pagination `<ul>`. Default: `null`.
- `PageChanging` (EventCallback<int>): Fired when a new page is selected; provides the target page index.

## Styling

The component renders a `<nav>` containing a `<ul class="pagination">`. It uses a `PagerItem` subcomponent for each item. Style it via your site CSS or a framework (Bootstrap classes are used by default in markup).

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

Add the component to any Razor component or page as shown above. Handle `PageChanging` to update your data and state.

## Notes

- The package targets modern .NET and is suitable for .NET 8+ Blazor apps.
- The component expects a minimal `IPage` contract with `PageIndex` and `TotalPageCount`. If your app uses a different paging model, provide a small adapter class or wrapper that exposes these properties.

## Contributing

Contributions are welcome. Please follow the repository's contribution guidelines and coding conventions.

## License

Specify the project license here.
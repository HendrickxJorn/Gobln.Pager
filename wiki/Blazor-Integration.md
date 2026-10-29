# Blazor Integration (`Gobln.Pager.Blazor`)

Targets `net8.0` and `net10.0`. Provides a single `<Pager>` component (namespace `Gobln.Pager.Blazor`) that renders Bootstrap-style pagination markup and raises an event when the user changes page — it does not fetch or hold data itself.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Page` | `IPage` | *(required)* | The page whose `PageIndex`/`TotalPageCount` drive the rendered controls. Throws `ArgumentNullException` if `null`. |
| `SiblingCount` | `int` | `1` | Number of page links shown on each side of the current page. |
| `BoundaryCount` | `int` | `0` | Number of page links always shown at the start and end of the range. |
| `ShowEllipsis` | `bool` | `false` | Whether to render a disabled `"..."` item where page numbers are skipped. An ellipsis that would hide a single page shows that page instead, the same as the MVC pager. |
| `ClassName` | `string?` | `null` | Extra CSS classes for the `<ul>`, added next to `pagination` (for example `pagination-lg justify-content-center`). |
| `ShowPager` | `WhenToShowEnum` | `Always` | When to render the pager at all. `IfNecessary`: only when there is more than one page. `Never`: nothing. |
| `ShowFirstButton`, `ShowPreviousButton`, `ShowNextButton`, `ShowLastButton` | `WhenToShowEnum` | `Always` | When to show each button. `IfNecessary`: First and Previous only when not on the first page, Next and Last only when not on the last page. `Always` shows them disabled where they can't go anywhere. |
| `FirstText`, `PreviousText`, `NextText`, `LastText` | `string` | `"First"`, `"Previous"`, `"Next"`, `"Last"` | The button texts. |
| `FirstIcon`, `PreviousIcon`, `NextIcon`, `LastIcon` | `RenderFragment?` | `null` | Shown instead of the text, which then becomes the button's `aria-label`. The icon itself is `aria-hidden`. |
| `AriaLabel` | `string` | `"Page navigation"` | The `aria-label` of the `<nav>`, the same default as the MVC pager. |
| `PageIndexChanged` | `EventCallback<int>` | — | Raised with the new page index when the user picks another page. Clicking the current page doesn't raise it. |

> The change event is `PageIndexChanged`, not `PageChanging`.

`Page` is typed as the non-generic `IPage` interface — pass any `Page<T>` (from the core library) or a custom `IPage` implementation. The component only reads `PageIndex` and `TotalPageCount` from it.

A `PagedList<T>` isn't an `IPage`. Pass its current page, and move the list when the user picks another page: `<Pager Page="list.GetPage()" PageIndexChanged="i => list.PageIndex = i" />`. `GetPage()` returns a new `Page<T>` on every render, which the pager always takes over, so it follows the list.

`WhenToShowEnum` (`Never`, `IfNecessary`, `Always`) is the Blazor package's own copy of the MVC package's enum, not a shared type. In a project that imports both namespaces, write `Gobln.Pager.Blazor.WhenToShowEnum`.

## Usage

```razor
@page "/items"
@using Gobln.Pager
@using Gobln.Pager.Blazor
@inject YourDbContext Context

<h3>Items</h3>

@if (currentPage is not null)
{
    <table class="table">
        <thead>
            <tr><th>ID</th><th>Name</th></tr>
        </thead>
        <tbody>
        @foreach (var item in currentPage)
        {
            <tr>
                <td>@item.Id</td>
                <td>@item.Name</td>
            </tr>
        }
        </tbody>
    </table>

    <Pager Page="currentPage"
           SiblingCount="1"
           BoundaryCount="1"
           ShowEllipsis="true"
           PageIndexChanged="OnPageIndexChanged" />
}
else
{
    <p><em>Loading...</em></p>
}

@code {
    private Page<YourEntity>? currentPage;
    private int pageIndex = 1;
    private const int PageSize = 10;

    protected override Task OnInitializedAsync() => LoadData();

    private async Task OnPageIndexChanged(int newPageIndex)
    {
        pageIndex = newPageIndex;
        await LoadData();
    }

    private async Task LoadData()
    {
        currentPage = await Context.YourEntities
            .OrderBy(e => e.Id)
            .ToPageAsync(pageIndex, PageSize);
    }
}
```

The pager reacts to clicks, so the component needs an interactive render mode, for example `@rendermode InteractiveServer`.

A `Page<T>` holds its items: `ToPageAsync` runs a count query and the query for the page once, and rendering the component doesn't query again. With [`Gobln.Pager.EntityFrameworkCore`](Extension-Methods.md#entity-framework-core-goblnpagerentityframeworkcore) installed, those queries run asynchronously (`CountAsync`, `ToListAsync`). Without it, `ToPageAsync` runs them synchronously on a thread-pool thread.

## Rendered markup

`<Pager>` renders:

```html
<nav aria-label="Page navigation">
  <ul class="pagination">
    <!-- First, Previous -->
    <!-- boundary page numbers (BoundaryCount) -->
    <!-- optional "..." -->
    <!-- sibling page numbers around PageIndex (SiblingCount) -->
    <!-- optional "..." -->
    <!-- boundary page numbers -->
    <!-- Next, Last -->
  </ul>
</nav>
```

Each entry is a `PagerItem` sub-component and follows Bootstrap 5.3's pagination markup:

- A page you can go to renders as `<li class="page-item"><a class="page-link" href="#">…</a></li>`.
- The active page is the same link, with `active` on the `<li>` and `aria-current="page"` on the link.
- A disabled entry ("First"/"Previous" on page 1, "Next"/"Last" on the last page, and the `"..."`) renders as `<li class="page-item disabled"><span class="page-link">…</span></li>`, without a click handler.
- The click handler sits on the `<a>` itself, next to its `preventDefault`, not on the `<li>`.
- A button with an icon renders the icon as `<span aria-hidden="true">…</span>`, with the button's text as the `aria-label` of its link or span.

## Testing reference

The test project (`test/Gobln.Pager.BlazorTest`) uses [bUnit](https://bunit.dev/) to render the component:

```csharp
var cut = Render<Pager>(parameters => parameters
    .Add(c => c.Page, testPage)
    .Add(c => c.PageIndexChanged, EventCallback.Factory.Create<int>(this, OnPageIndexChanged)));
```

See `PagerComponentTest01.cs` and `PagerComponentTest02.cs`, and the minimal `IPage` test double in `test/Gobln.Pager.BlazorTest/Helpers/TestPage.cs`, for further examples.

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
| `PageIndexChanged` | `EventCallback<int>` | — | Raised with the new page index when the user picks another page. Clicking the current page doesn't raise it. |

> The change event is `PageIndexChanged`, not `PageChanging`.

`Page` is typed as the non-generic `IPage` interface — pass any `Page<T>` (from the core library) or a custom `IPage` implementation. The component only reads `PageIndex` and `TotalPageCount` from it.

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

A `Page<T>` made from an `IQueryable<T>` runs its query each time it is enumerated, so the component above queries the database on every render. To query once per page change, load the rows yourself and wrap them in a pre-paged page:

```csharp
private async Task LoadData()
{
    var query = Context.YourEntities.OrderBy(e => e.Id);
    var totalCount = await query.CountAsync();
    var rows = await query.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToListAsync();

    currentPage = rows.ToPage(pageIndex, PageSize, totalCount);
}
```

## Rendered markup

`<Pager>` renders:

```html
<nav aria-label="page navigation">
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

## Testing reference

The test project (`test/Gobln.Pager.BlazorTest`) uses [bUnit](https://bunit.dev/) to render the component:

```csharp
var cut = Render<Pager>(parameters => parameters
    .Add(c => c.Page, testPage)
    .Add(c => c.PageIndexChanged, EventCallback.Factory.Create<int>(this, OnPageIndexChanged)));
```

See `PagerComponentTest01.cs` and `PagerComponentTest02.cs`, and the minimal `IPage` test double in `test/Gobln.Pager.BlazorTest/Helpers/TestPage.cs`, for further examples.

# Core Library (`Gobln.Pager`)

All types below live in the `Gobln.Pager` namespace, in the core `Gobln.Pager` package.

## `Page<T>` — single page snapshot

A read-only, strongly-typed snapshot of one page of a larger collection. Implements `IPage<T>` and `IEnumerable<T>`, and is `[Serializable]`.

### Properties

| Property | Type | Description |
|---|---|---|
| `PageIndex` | `int` | Current page number (1-based). Default `1`. |
| `PageSize` | `int` | Number of items in this page. Default `0`. |
| `TotalItemCount` | `int` | Total items in the original collection. Default `0`. |
| `TotalPageCount` | `int` | Total pages in the original collection. Default `1`. |
| `HasPaging` | `bool` | `TotalPageCount > 1`. |
| `IsEmpty` | `bool` | `true` if the page contains no items. |

All properties are read-only (`{ get; private set; }`) once the page is constructed.

### Constructors

```csharp
Page()                                                                 // empty page
Page(IEnumerable<T> collection)                                       // whole collection as a single page
Page(IEnumerable<T> collection, int pageIndex, int pageSize, int totalItemCount)  // pre-paged data
```

The three-argument-plus form is for **pre-paged data**: `collection` is assumed to already be exactly the items for `pageIndex`, and is *not* re-sliced. It throws `ArgumentOutOfRangeException` if `pageIndex < 1`, `pageSize < 1`, or `totalItemCount < 0`.

```csharp
var prePagedData = new List<string> { "Item 1", "Item 2", "Item 3" };
var page = new Page<string>(prePagedData, pageIndex: 1, pageSize: 3, totalItemCount: 3);
```

### `Page` — static factory helper

A separate, non-generic `Page` class (implementing `IPage` with plain mutable properties) exposes static factory methods:

```csharp
Page<T> Page.FromEnumerable<T>(IEnumerable<T> source);
Page<T> Page.FromEnumerable<T>(IEnumerable<T> source, int pageIndex, int pageSize);
Page<T> Page.FromEnumerable<T>(IEnumerable<T> source, int pageIndex, int pageSize, int totalItemCount);
```

In most code you'll use the [`ToPage()` extension methods](Extension-Methods.md) instead of calling these factories directly.

---

## `PagedList<T>` — mutable, navigable collection

A mutable collection that maintains its own paging state. Implements `IPagedList<T>`, `IReadOnlyList<T>`, and is `[Serializable]`.

### Constructors

```csharp
PagedList()                                    // empty, page size 10
PagedList(int pageSize)                        // empty, given page size
PagedList(IEnumerable<T> collection)            // seeded, page size 10
PagedList(IEnumerable<T> collection, int pageSize)  // seeded, given page size
```

> There is **no** 3-argument `PagedList<T>(pageIndex, pageSize, totalCount)` constructor — `PagedList<T>` computes its own page count from the items it holds. To create one from an existing sequence, use `PagedList(collection, pageSize)` or the `ToPagedList()` extension method.

```csharp
var pagedList = new PagedList<int>(Enumerable.Range(1, 50), pageSize: 10);
```

### Properties

| Property | Type | Description |
|---|---|---|
| `PageIndex` | `int` | Current page (get/set). Setting clamps to `[1, PageCount]`. |
| `PageSize` | `int` | Items per page (get/set). Setting clamps to `>= 1` and recalculates `PageCount`. |
| `PageCount` | `int` | Total number of pages (read-only). |
| `HasNextPage` | `bool` | `PageIndex < PageCount`. |
| `HasPreviousPage` | `bool` | `PageIndex > 1`. |
| `Count` | `int` | Total items across all pages. |
| `IsReadOnly` | `bool` | Always `false`. |
| `this[int index]` | `T` | Indexer into the full underlying list (not the current page). |

### Methods

| Method | Description |
|---|---|
| `Add(T item)` | Append an item; recalculates paging. |
| `AddRange(IEnumerable<T> collection)` | Append many items; recalculates paging. |
| `Insert(int index, T item)` | Insert at index; recalculates paging. |
| `Remove(T item)` / `RemoveAt(int index)` | Remove an item; recalculates paging. |
| `Clear()` | Remove all items. |
| `Contains(T item)` / `IndexOf(T item)` / `CopyTo(T[], int)` | Standard collection operations. |
| `GetPage(int pageIndex)` | Returns a `Page<T>` snapshot of the given page. |
| `GetPage()` | Returns a `Page<T>` snapshot of the *current* page (`PageIndex`). |
| `GetPages()` | Yields a `Page<T>` for every page, in order. |
| `NextPage()` | Advances `PageIndex` by 1 (clamped) and returns `this`, for chaining. |
| `PreviousPage()` | Moves `PageIndex` back by 1 (clamped) and returns `this`, for chaining. |
| `AsReadOnly()` | Returns a `ReadOnlyCollection<T>` wrapper over the current items. |

> The navigation methods are named **`NextPage()`** / **`PreviousPage()`** — not `GoToNextPage()` / `GoToPreviousPage()`.

```csharp
var pagedList = new PagedList<int>(Enumerable.Range(1, 50), pageSize: 10);

pagedList.Add(51);
pagedList.Insert(0, 0);
pagedList.Remove(25);

var currentPage = pagedList.GetPage();
Console.WriteLine($"Page {pagedList.PageIndex} of {pagedList.PageCount}, {pagedList.Count} items total.");

pagedList.NextPage();
var nextPage = pagedList.GetPage();
```

### `PagedList.FromEnumerable(...)`

The static `PagedList` class exposes an extension member used internally by `ToPagedList()`:

```csharp
PagedList<T> PagedList.FromEnumerable<T>(this IEnumerable<T> source, int pageSize = 10);
```

Prefer the [`ToPagedList()` extension methods](Extension-Methods.md) in application code.

---

## `PageFilter` — pagination parameter DTO

A simple settable DTO implementing `IPageFilter`, useful for binding pagination parameters from a request (e.g. an MVC action or API query string).

```csharp
public class PageFilter : IPageFilter
{
    public PageFilter();
    public PageFilter(int pageIndex, int pageSize);

    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
```

```csharp
var filter = new PageFilter(pageIndex: 2, pageSize: 25);
var page = items.ToPage(filter);
```

---

## `PageWindow` — page numbers for a pager

A static helper that works out which page numbers a pager shows. The [Blazor `Pager` component](Blazor-Integration.md) and the [MVC `Html.Pager()` helper](Mvc-Integration.md) both render what it returns, and a pager you render yourself can use it too.

```csharp
IReadOnlyList<int?> PageWindow.GetPageNumbers(int pageIndex, int totalPageCount, int siblingCount, int boundaryCount, bool showEllipsis);
```

| Parameter | Description |
|---|---|
| `pageIndex` | The current page. |
| `totalPageCount` | Total number of pages. |
| `siblingCount` | Page numbers shown on each side of the current page. Below `0` counts as `0`. |
| `boundaryCount` | Page numbers always shown at the start and at the end. Below `0` counts as `0`. |
| `showEllipsis` | Mark the places where page numbers are skipped. |

The result holds the page numbers in order, with `null` where an ellipsis goes. With `showEllipsis`, the list has the same number of entries on every page, and an ellipsis that would hide a single page shows that page instead.

```csharp
PageWindow.GetPageNumbers(pageIndex: 5, totalPageCount: 10, siblingCount: 1, boundaryCount: 1, showEllipsis: true);
// 1, null, 4, 5, 6, null, 10

PageWindow.GetPageNumbers(pageIndex: 4, totalPageCount: 10, siblingCount: 1, boundaryCount: 1, showEllipsis: true);
// 1, 2, 3, 4, 5, null, 10

PageWindow.GetPageNumbers(pageIndex: 5, totalPageCount: 10, siblingCount: 1, boundaryCount: 1, showEllipsis: false);
// 1, 4, 5, 6, 10
```

---

## Validation rules

Pagination parameters are validated wherever they're accepted (constructors, extension methods):

- `pageIndex < 1` → throws `ArgumentOutOfRangeException`
- `pageSize < 1` → throws `ArgumentOutOfRangeException`
- `totalItemCount < 0` → throws `ArgumentOutOfRangeException`

## Interfaces

| Interface | Implemented by |
|---|---|
| `IPage` | `Page` (non-generic), and inherited by `IPage<T>` |
| `IPage<T>` | `Page<T>` |
| `IPagedList` | inherited by `IPagedList<T>` |
| `IPagedList<T>` | `PagedList<T>` |
| `IPageFilter` | `PageFilter` |

`IPage` (non-generic) is what the [Blazor `Pager` component](Blazor-Integration.md) and [MVC `Html.Pager()` helper](Mvc-Integration.md) accept, since neither needs to know the item type to render pagination controls.

## Thread safety

- **`Page<T>`**: thread-safe after construction (read-only).
- **`PagedList<T>`**: **not** thread-safe — use external locking or a concurrent wrapper in multi-threaded scenarios.
- **Extension methods**: thread-safe (stateless).

# Extension Methods

The core package adds `ToPage(...)` / `ToPagedList(...)` extension methods (plus async equivalents) to both `IEnumerable<T>` and `IQueryable<T>`. The overload sets are identical on both interfaces; the difference is *how* the page is materialized:

- On **`IEnumerable<T>`**, paging happens in memory (`Skip`/`Take` over the sequence).
- On **`IQueryable<T>`**, paging is translated into the query provider (e.g. Entity Framework turns `Skip`/`Take` into SQL) — only the requested page is pulled into memory, *except* for `ToPagedList()`, see the warning below. Sort the query before paging it: without an `OrderBy` the database picks the order, and one page can repeat items of another.

Either way the returned page holds its items: the source is read once, by `ToPage`, and not again when the page is enumerated; see [Core Library](Core-Library.md#paget--one-page-of-items).

## `ToPage(...)` overloads

```csharp
Page<T> ToPage();
Page<T> ToPage(int pageIndex, int pageSize);
Page<T> ToPage(IPageFilter pagerFilter);
Page<T> ToPage(int pageIndex, int pageSize, int totalItemCount);                 // pre-paged
Page<T> ToPage(IPageFilter pagerFilter, int totalItemCount);                     // pre-paged
Page<T> ToPage(int pageIndex, int pageSize, int totalItemCount, bool prePaged);
Page<T> ToPage(IPageFilter pagerFilter, int totalItemCount, bool prePaged);
```

- `ToPage()` with no arguments returns the entire sequence as a single page.
- Overloads that take `totalItemCount` without a `prePaged` flag treat the source as **already paged** (`prePaged: true`) — i.e. they assume `source` is exactly the slice for `pageIndex`, and do not re-slice it. This is the fast path for data you've already paged at the database level.
- The `prePaged` flag lets you be explicit either way.
- A `pageIndex` past the last page returns the last page. A pre-paged source that came back empty keeps its `TotalItemCount` and `TotalPageCount`.

```csharp
// In-memory paging
var inMemoryPage = items.ToPage(pageIndex: 1, pageSize: 10);

// Entity Framework — Skip/Take happen in SQL
var databasePage = context.MyEntities.OrderBy(e => e.Id).ToPage(pageIndex: 2, pageSize: 5);

// Pre-paged: you already ran the query with your own OFFSET/LIMIT and COUNT
var alreadyPagedRows = await context.MyEntities
    .OrderBy(e => e.Id)
    .Skip((pageIndex - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
var totalCount = await context.MyEntities.CountAsync();

var prePagedPage = alreadyPagedRows.ToPage(pageIndex, pageSize, totalCount); // prePaged: true implicitly
```

## `ToPagedList(...)` overloads

```csharp
PagedList<T> ToPagedList();
PagedList<T> ToPagedList(int pageSize);
```

> **Warning:** on `IQueryable<T>`, `ToPagedList()` executes the entire query and loads all results into memory (this is documented directly on the method). Use `ToPage(...)` instead when you only need one page from a large/database-backed source.

## Async variants

Every overload above has an async twin with the same name plus `Async`, taking an optional trailing `CancellationToken`:

```csharp
Task<Page<T>> ToPageAsync(CancellationToken cancellationToken = default);
Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, CancellationToken cancellationToken = default);
Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, int totalItemCount, CancellationToken cancellationToken = default);
Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, int totalItemCount, CancellationToken cancellationToken = default);
Task<Page<T>> ToPageAsync(int pageIndex, int pageSize, int totalItemCount, bool prePaged, CancellationToken cancellationToken = default);
Task<Page<T>> ToPageAsync(IPageFilter pagerFilter, int totalItemCount, bool prePaged, CancellationToken cancellationToken = default);

Task<PagedList<T>> ToPagedListAsync(CancellationToken cancellationToken = default);
Task<PagedList<T>> ToPagedListAsync(int pageSize, CancellationToken cancellationToken = default);
```

> There is **no** `ToPagedResultAsync` method — the real names are `ToPageAsync` and `ToPagedListAsync`.

The core package declares these on `IEnumerable<T>` and runs the synchronous logic on a thread-pool thread (`Task.Run`). On an `IQueryable<T>` they still page in the query: they notice the query at runtime and use the `IQueryable<T>` `ToPage`. `AsEnumerable()` returns the same object, so `query.AsEnumerable().ToPageAsync(...)` pages in the query too.

### Entity Framework Core: `Gobln.Pager.EntityFrameworkCore`

`dotnet add package Gobln.Pager.EntityFrameworkCore` adds the same async overloads on `IQueryable<T>`, in the same `Gobln.Pager` namespace, so existing calls pick them up without code changes. On an EF Core query they run `CountAsync` and `ToListAsync` with your `CancellationToken`, instead of blocking a thread-pool thread:

| Call | Queries |
| --- | --- |
| `ToPageAsync(pageIndex, pageSize)`, `ToPageAsync(pageFilter)` | `CountAsync`, then the page with `Skip`/`Take` (no second query when the count is 0) |
| `ToPageAsync(pageIndex, pageSize, totalItemCount)` (pre-paged) | `ToListAsync` of the query as it is (none when the total is 0) |
| `ToPageAsync(..., totalItemCount, prePaged: false)` | the page with `Skip`/`Take`, no count |
| `ToPageAsync()`, `ToPagedListAsync(...)` | one `ToListAsync` |

On a query that isn't from EF Core, such as `list.AsQueryable()`, they do what the core package does. The package targets net8.0 (EF Core 8.0.20 or later) and net10.0 (EF Core 10.0.1 or later).

```csharp
var page = await context.MyEntities
    .OrderBy(e => e.Id)
    .ToPageAsync(pageIndex: 1, pageSize: 10, cancellationToken);
```

## `PageFilter` as a parameter object

Any overload accepting `IPageFilter` can take the built-in [`PageFilter`](Core-Library.md#pagefilter--pagination-parameter-dto) or your own implementation — handy for binding pagination straight from a request model:

```csharp
public class MyQueryModel : IPageFilter
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    // ...other filter fields
}

var page = items.ToPage(model);
```

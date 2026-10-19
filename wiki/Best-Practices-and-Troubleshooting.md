# Best Practices & Troubleshooting

## Best Practices

- **Prefer `IQueryable<T>.ToPage(...)` over `IQueryable<T>.ToPagedList()` for large/database-backed sources.** `ToPage(pageIndex, pageSize)` translates to `Skip`/`Take` in the query provider, fetching only one page. `ToPagedList()` on `IQueryable<T>` executes the *entire* query into memory first — only use it on sources you already know are small, or on `IEnumerable<T>` collections that are already in memory.
- **With Entity Framework Core, install `Gobln.Pager.EntityFrameworkCore` for async paging.** The core package's `ToPageAsync` runs the queries synchronously on a thread-pool thread. With the EF Core package, the same calls run `CountAsync` and `ToListAsync`.
- **Use pre-paged overloads (`prePaged: true`, or the `totalItemCount`-only overloads) when you've already paged and counted at the data-access layer** — e.g. you ran your own `Skip`/`Take` plus a separate `Count()` query. This avoids double-paging the data.
- **Use `PagedList<T>` when the collection needs to be mutated after paging** (add/remove/insert) and you want the page count to stay in sync automatically. Use `Page<T>` for a single page to display or return from an API.
- **Sort an `IQueryable<T>` before paging it.** Without an `OrderBy` the database picks the order, so pages can repeat or skip items.
- **Validate pagination input at your application boundary** (controller/API parameters) before calling into the library — the library will throw `ArgumentOutOfRangeException` for `pageIndex < 1`, `pageSize < 1`, or `totalItemCount < 0`, but a friendlier validation message at the boundary is usually better UX than an unhandled exception. A `pageIndex` past the last page doesn't throw: it returns the last page.
- **`PagedList<T>` is not thread-safe.** Guard concurrent access yourself if a `PagedList<T>` instance is shared across requests/threads.
- **Pick one JSON package's wire format and stick to it end-to-end** if `Page<T>` payloads cross a service boundary — see the interop note in [JSON Serialization](Json-Serialization.md).

## Troubleshooting

### Pagination doesn't behave as expected with Entity Framework `IQueryable<T>`

Make sure you're calling `ToPage`/`ToPageAsync` (which apply `Skip`/`Take` to the query before it executes) rather than materializing the whole query first with `ToList()`/`ToListAsync()` and paging afterward — the latter defeats database-side paging entirely and pages in memory instead.

If pages repeat or skip items, the query probably isn't sorted. Add an `OrderBy` on a unique key (or end the sort with one) before `ToPage`.

### `ObjectDisposedException` when a page from Entity Framework is shown

Since 2.0 a `Page<T>` reads its rows when `ToPage` runs and keeps them, so enumerating the page later (in a view, after a `using` disposed the `DbContext`) no longer touches the context. Before 2.0 the page ran its query on every enumeration, which threw `ObjectDisposedException: Cannot access a disposed context instance` in that case. If you still see it, `ToPage` itself ran on a disposed context: make the page while the `DbContext` is open. A `PagedList<T>` from `ToPagedList()` loads everything up front and was never affected.

### `ArgumentOutOfRangeException` when creating a `Page<T>` or calling `ToPage(...)`

Check that `pageIndex >= 1`, `pageSize >= 1`, and (where applicable) `totalItemCount >= 0`. These are validated on every entry point (constructors and extension methods).

### `ArgumentNullException: Page parameter is required` in the Blazor `<Pager>` component

The `Page` parameter on `<Pager>` is `[EditorRequired]` and the component throws if it's `null` in `OnParametersSet`. Ensure the bound `IPage`/`Page<T>` value is assigned (e.g. after an initial data load completes) before the component first renders, or guard the markup with `@if (page is not null)`.

### `PagedList<T>` doesn't have a `(pageIndex, pageSize, totalItemCount)` constructor

That constructor doesn't exist — `PagedList<T>` computes its own paging state from the items it holds. Use `new PagedList<T>(collection, pageSize)`, or `collection.ToPagedList(pageSize)`. For a one-off pre-paged snapshot instead, use `Page<T>` via `new Page<T>(items, pageIndex, pageSize, totalItemCount)`.

### Looking for `ToPagedResultAsync`, `GoToNextPage()`/`GoToPreviousPage()`, or a `PageChanging` Blazor parameter

These names don't exist in the library and were mistakes in earlier documentation. The real names are:

| Looked for | Actual API |
| --- | --- |
| `ToPagedResultAsync` | `ToPageAsync` / `ToPagedListAsync` (see [Extension Methods](Extension-Methods.md)) |
| `PagedList<T>.GoToNextPage()` | `PagedList<T>.NextPage()` |
| `PagedList<T>.GoToPreviousPage()` | `PagedList<T>.PreviousPage()` |
| `<Pager PageChanging="...">` | `<Pager PageIndexChanged="...">` |

### Getting help

- **Documentation:** this wiki reflects the source at the time of writing; when in doubt, the source under `scr/` is authoritative.
- **GitHub Issues:** [github.com/HendrickxJorn/Gobln.Pager/issues](https://github.com/HendrickxJorn/Gobln.Pager/issues)

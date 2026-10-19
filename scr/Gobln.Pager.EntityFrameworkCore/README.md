# Gobln.Pager.EntityFrameworkCore

[![NuGet](https://img.shields.io/nuget/dt/Gobln.Pager.EntityFrameworkCore.svg)](https://www.nuget.org/packages/Gobln.Pager.EntityFrameworkCore/)
[![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.EntityFrameworkCore.svg)](https://www.nuget.org/packages/Gobln.Pager.EntityFrameworkCore/)

Asynchronous paging for Entity Framework Core with [Gobln.Pager](https://www.nuget.org/packages/Gobln.Pager/). On an EF Core query, `ToPageAsync` and `ToPagedListAsync` run `CountAsync` and `ToListAsync`. Without this package they run the same queries synchronously, on a thread-pool thread.

## Frameworks

- .NET 8.0, with Entity Framework Core 8.0.32 or later
- .NET 10.0, with Entity Framework Core 10.0.13 or later

## Installation

```shell
dotnet add package Gobln.Pager.EntityFrameworkCore
```

## Usage

Nothing changes in your code. The methods are in the `Gobln.Pager` namespace, so the calls you already have pick them up:

```csharp
using Gobln.Pager;

var page = await context.Orders
    .OrderBy(o => o.Id)
    .ToPageAsync(pageIndex: 2, pageSize: 10, cancellationToken);
```

| Call | Queries |
| --- | --- |
| `ToPageAsync(pageIndex, pageSize)`, `ToPageAsync(pageFilter)` | `CountAsync`, then the page with `Skip`/`Take`. No second query when the count is 0. |
| `ToPageAsync(pageIndex, pageSize, totalItemCount)` (pre-paged) | `ToListAsync` of the query as it is. None when the total is 0. |
| `ToPageAsync(pageIndex, pageSize, totalItemCount, prePaged: false)` | The page with `Skip`/`Take`, no count |
| `ToPageAsync()`, `ToPagedListAsync()`, `ToPagedListAsync(pageSize)` | One `ToListAsync` |

The `CancellationToken` goes to Entity Framework, so it cancels a query that is running. A page index past the last page returns the last page, as `ToPage` does.

## Behaviour to know about

- Sort the query before paging it. Without an `OrderBy` the database picks the order, and one page can repeat items of another.
- On a query that isn't from Entity Framework Core, such as `list.AsQueryable()` or a mocked `DbSet`, the methods do what the core package does.
- `ToPagedListAsync` reads the whole query.

## Documentation

The [wiki](https://github.com/HendrickxJorn/Gobln.Pager/tree/master/wiki) describes every type and extension method.

## License

[MIT](https://github.com/HendrickxJorn/Gobln.Pager/blob/master/LICENSE.md)

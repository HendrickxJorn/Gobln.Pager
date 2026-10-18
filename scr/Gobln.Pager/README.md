# Gobln.Pager

[![NuGet](https://img.shields.io/nuget/dt/Gobln.Pager.svg)](https://www.nuget.org/packages/Gobln.Pager/)
[![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.svg)](https://www.nuget.org/packages/Gobln.Pager/)

A paging library for .NET. It turns an `IEnumerable<T>` or `IQueryable<T>` into a `Page<T>`: the items of one page, together with the page index, page size and totals a pager needs. For a list that keeps changing there is `PagedList<T>`, which pages itself. The package has no dependencies.

Related packages:

- [Gobln.Pager.Mvc](https://www.nuget.org/packages/Gobln.Pager.Mvc/) renders a pager in ASP.NET Core and ASP.NET MVC 5 views.
- [Gobln.Pager.Blazor](https://www.nuget.org/packages/Gobln.Pager.Blazor/) is a pager component for Blazor.
- [Gobln.Pager.Json.Microsoft](https://www.nuget.org/packages/Gobln.Pager.Json.Microsoft/) and [Gobln.Pager.Json.Newtonsoft](https://www.nuget.org/packages/Gobln.Pager.Json.Newtonsoft/) read and write pages as JSON.

## Frameworks

- .NET 8.0 and .NET 10.0
- .NET Standard 2.0 and 2.1
- .NET Framework 4.6.2 and 4.8

## Installation

```shell
dotnet add package Gobln.Pager
```

## Page

`ToPage(pageIndex, pageSize)` takes one page out of a collection. The first page is page 1.

```csharp
using Gobln.Pager;

List<Order> orders = GetOrders();   // 95 orders

var page = orders.ToPage(pageIndex: 2, pageSize: 10);

foreach (var order in page)
{
    // orders 11 to 20
}

// page.PageIndex        2
// page.PageSize         10
// page.TotalItemCount   95
// page.TotalPageCount   10
// page.HasPaging        true: there is more than one page
// page.IsEmpty          false
```

On an `IQueryable<T>`, such as an Entity Framework query, the paging is added to the query, so only the items of that page are fetched. Sort the query before paging it: without an `OrderBy` the database picks the order, and one page can repeat items of another.

`ToPage()` without arguments puts the whole collection on a single page.

### Page index and size from a request

`PageFilter` carries the page index and the page size as one object, for example as an action parameter that model binding fills:

```csharp
var page = orders.ToPage(new PageFilter(pageIndex: 2, pageSize: 10));
```

A class of your own works too, when it implements `IPageFilter`.

### Data that is already paged

When the data source already returned a single page (a stored procedure, another API), pass the total item count along. The collection is then used as it is:

```csharp
// rows holds only the 10 items of page 3
var page = rows.ToPage(pageIndex: 3, pageSize: 10, totalItemCount: 95);
```

### Async

Every `ToPage` and `ToPagedList` has an `Async` version that takes a `CancellationToken`:

```csharp
var page = await orders.ToPageAsync(2, 10, cancellationToken);
```

## PagedList

A `PagedList<T>` is a list that keeps its own page size and current page. Items can be added and removed, and the page count follows.

```csharp
var list = orders.ToPagedList(pageSize: 10);    // or: new PagedList<Order>(pageSize: 10)

list.Add(new Order());                          // list.PageCount is recalculated

list.PageIndex = 2;

var page = list.GetPage();                      // the current page, as a Page<Order>
page = list.GetPage(1);                         // any other page; PageIndex stays 2

list.NextPage();                                // PageIndex is now 3
list.PreviousPage();                            // and 2 again

page = list.NextPage().GetPage();               // both return the list, so they chain

foreach (var pageOfOrders in list.GetPages())
{
    // every page, in order
}
```

`PageIndex` always stays between 1 and `PageCount`: a lower value becomes 1, a higher value becomes the last page. `HasNextPage` and `HasPreviousPage` tell whether there is a page to go to.

## Page numbers for your own pager

`PageWindow.GetPageNumbers` works out which page numbers a pager shows. The MVC and Blazor pagers use it, and so can a pager you render yourself:

```csharp
var numbers = PageWindow.GetPageNumbers(pageIndex: 5, totalPageCount: 10, siblingCount: 1, boundaryCount: 1, showEllipsis: true);

// 1, null, 4, 5, 6, null, 10
```

`null` marks an ellipsis. `siblingCount` is the number of pages on each side of the current page, `boundaryCount` the number of pages always shown at the start and at the end.

## Behaviour to know about

- A `Page<T>` reads its items from the source each time it is enumerated. On an `IQueryable<T>`, every `foreach` over the page, and `IsEmpty`, runs the query again, so the `DbContext` has to stay open until the page has been shown.
- A page index or page size below 1 throws `ArgumentOutOfRangeException`. So does a total item count below 0.
- `ToPage(pageIndex, pageSize)` with a page index past the last page returns an empty page whose `TotalItemCount` is 0 and whose `TotalPageCount` is 1. When the index comes from a request, keep it within the pages there are before calling `ToPage`.
- `ToPagedList` on an `IQueryable<T>` runs the whole query and loads the result in memory.
- The `Async` versions run the same work on a thread-pool thread. They don't use Entity Framework's async queries.

## Upgrading from version 1

Support for .NET Framework 4.5 and lower, and for .NET Standard 1.6, is removed.

### Changes to Page

- `CurrentPageIndex` is renamed to `PageIndex`.
- `PageIndex` is 1 by default, not 0.
- `ToPage` with a total item count now treats the collection as already paged. To have the collection sliced, as in version 1, pass `prePaged: false` as the last argument.
- `PagerFilter` and `IPagerFilter` are renamed to `PageFilter` and `IPageFilter`.

### Changes to PagedList

- `CurrentPageIndex` is renamed to `PageIndex`.
- `GetCurrentPage()` is renamed to `GetPage()`.
- `Next()` is renamed to `NextPage()`, and `Previous()` to `PreviousPage()`.
- `SetPageIndex()`, `GetNextPage()`, `GetPreviousPage()`, `PeakNextPage()` and `PeakPreviousPage()` are removed. Use `NextPage()` or `PreviousPage()`, followed by `GetPage()`.

## Documentation

The [wiki](https://github.com/HendrickxJorn/Gobln.Pager/tree/master/wiki) describes every type and extension method.

## Issues and contributions

Found a bug, or something to improve? Open an issue or send a pull request on [GitHub](https://github.com/HendrickxJorn/Gobln.Pager).

## License

[MIT](https://github.com/HendrickxJorn/Gobln.Pager/blob/master/LICENSE.md)

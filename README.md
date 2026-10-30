# Gobln.Pager

| Gobln.Pager | Gobln.Pager.EntityFrameworkCore | Gobln.Pager.Mvc | Gobln.Pager.Blazor | Gobln.Pager.Json.Microsoft | Gobln.Pager.Json.Newtonsoft |
| --- | --- | --- | --- | --- | --- |
| [![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.svg)](https://www.nuget.org/packages/Gobln.Pager/) | [![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.EntityFrameworkCore.svg)](https://www.nuget.org/packages/Gobln.Pager.EntityFrameworkCore/) | [![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.Mvc.svg)](https://www.nuget.org/packages/Gobln.Pager.Mvc/) | [![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.Blazor.svg)](https://www.nuget.org/packages/Gobln.Pager.Blazor/) | [![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.Json.Microsoft.svg)](https://www.nuget.org/packages/Gobln.Pager.Json.Microsoft/) | [![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.Json.Newtonsoft.svg)](https://www.nuget.org/packages/Gobln.Pager.Json.Newtonsoft/) |

A portable, zero-dependency paging library for .NET. It turns `IEnumerable<T>` and `IQueryable<T>` collections into strongly-typed paginated results, with optional packages for Entity Framework Core, Blazor, ASP.NET MVC/Core, and JSON serialization.

```csharp
using Gobln.Pager;

var page = context.Products.OrderBy(p => p.Name).ToPage(pageIndex: 2, pageSize: 10);

Console.WriteLine($"Page {page.PageIndex} of {page.TotalPageCount}, {page.TotalItemCount} items total.");

foreach (var product in page)
{
    Console.WriteLine(product.Name);
}
```

On an `IQueryable<T>` the paging is translated into the query, so only the requested page is fetched.

## Why Gobln.Pager?

- **Framework agnostic** — .NET Framework 4.6.2/4.8, .NET Standard 2.0/2.1, and .NET 8/10
- **LINQ integration** — works on `IEnumerable<T>` and `IQueryable<T>` (including Entity Framework), synchronously or asynchronously, with EF Core's own async queries through `Gobln.Pager.EntityFrameworkCore`
- **Pre-paged support** — wrap data you already paged and counted at the database level, without re-slicing it
- **Ready-made UI** — a Blazor component and an ASP.NET MVC/Core HTML helper
- **JSON support** — converters for both `System.Text.Json` and `Newtonsoft.Json`, sharing one wire format
- **No external dependencies** in the core package

## Packages

| Package | Target Frameworks | Purpose |
| --- | --- | --- |
| `Gobln.Pager` | net462, net48, net8.0, net10.0, netstandard2.0, netstandard2.1 | Core paging types and LINQ extension methods |
| `Gobln.Pager.EntityFrameworkCore` | net8.0, net10.0 | Asynchronous `ToPageAsync` / `ToPagedListAsync` with EF Core's `CountAsync` / `ToListAsync` |
| `Gobln.Pager.Blazor` | net8.0, net10.0 | `<Pager>` Razor component |
| `Gobln.Pager.Mvc` | net462, net48, net8.0, net10.0 | `Html.Pager()` helper for ASP.NET MVC / ASP.NET Core |
| `Gobln.Pager.Json.Microsoft` | net462, net48, net8.0, net10.0, netstandard2.0, netstandard2.1 | `System.Text.Json` converters |
| `Gobln.Pager.Json.Newtonsoft` | net462, net48, net8.0, net10.0, netstandard2.0, netstandard2.1 | `Newtonsoft.Json` converters |

Install only what you need — the core package is required by all of the others.

```bash
dotnet add package Gobln.Pager
```

## The two page types

**`Page<T>`** is one page of items, carrying `PageIndex`, `PageSize`, `TotalItemCount`, `TotalPageCount`, `HasPaging`, and `IsEmpty`. It holds its items, read once when the page is made, as an `IReadOnlyList<T>` (`Count`, `page[0]`). Every page also has `HasPreviousPage()`, `HasNextPage()`, `FirstItemOnPage()` and `LastItemOnPage()`. Create one with `ToPage(...)`:

```csharp
var page = items.ToPage(pageIndex: 1, pageSize: 10);      // page a collection
page = items.ToPage(new PageFilter(pageIndex, pageSize)); // from bound request parameters
page = rows.ToPage(pageIndex, pageSize, totalItemCount);  // data already paged in the database
page = await items.ToPageAsync(1, 10, cancellationToken);
```

**`PagedList<T>`** is a mutable, navigable collection that keeps its own paging state — use it when the collection changes after paging:

```csharp
var list = items.ToPagedList(pageSize: 10);

list.Add(newItem);        // page count recalculates as the list changes
list.NextPage();

var current = list.GetPage();   // Page<T> for list.PageIndex
```

## Blazor

```razor
@using Gobln.Pager.Blazor

<Pager Page="currentPage"
       SiblingCount="1"
       BoundaryCount="1"
       ShowEllipsis="true"
       PageIndexChanged="OnPageIndexChanged" />
```

The component renders Bootstrap-style pagination markup and raises `PageIndexChanged` with the new index; loading the data stays with your code.

## ASP.NET MVC / ASP.NET Core

```cshtml
@using Gobln.Pager.Mvc
@model Page<Product>

@Html.Pager(Model)
```

Register defaults once at startup, or pass a `PagerOptions` per call:

```csharp
builder.Services.AddMvcPager(options => options.SiblingCount = 2);
```

## JSON

```csharp
var options = new JsonSerializerOptions();
options.Converters.Add(new Gobln.Pager.Json.Microsoft.PageJsonConverter());
options.Converters.Add(new Gobln.Pager.Json.Microsoft.PagedListJsonConverter());
```

Both JSON packages write the same shape, so a `System.Text.Json` API and a `Newtonsoft.Json` client interoperate directly.

## Documentation

Full documentation lives in [`wiki/`](wiki/):

| Page | Contents |
| --- | --- |
| [Getting Started](wiki/Getting-Started.md) | Installation, prerequisites, minimal example |
| [Core Library](wiki/Core-Library.md) | `Page<T>`, `PagedList<T>`, `PageFilter`, `PageWindow`, interfaces |
| [Extension Methods](wiki/Extension-Methods.md) | Every `ToPage` / `ToPagedList` overload and its async twin |
| [Blazor Integration](wiki/Blazor-Integration.md) | The `<Pager>` component |
| [MVC / ASP.NET Core Integration](wiki/Mvc-Integration.md) | `Html.Pager()` and `PagerOptions` |
| [JSON Serialization](wiki/Json-Serialization.md) | Converters and the shared wire format |
| [Best Practices & Troubleshooting](wiki/Best-Practices-and-Troubleshooting.md) | Recommendations and common pitfalls |

## License

[MIT](LICENSE.md)

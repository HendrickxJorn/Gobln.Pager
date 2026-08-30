# Getting Started

## Prerequisites

- .NET Framework 4.6.2+, .NET Standard 2.0+, or .NET 6+
- Visual Studio 2015+ or any .NET-compatible IDE

## Installing packages

Install only the packages you need. The core package is required by all others.

```bash
# Core paging library (always required)
dotnet add package Gobln.Pager

# Optional: Blazor <Pager> component
dotnet add package Gobln.Pager.Blazor

# Optional: ASP.NET MVC / ASP.NET Core Html.Pager() helper
dotnet add package Gobln.Pager.Mvc

# Optional: JSON converters for System.Text.Json
dotnet add package Gobln.Pager.Json.Microsoft

# Optional: JSON converters for Newtonsoft.Json
dotnet add package Gobln.Pager.Json.Newtonsoft
```

Or via the Package Manager Console:

```powershell
Install-Package Gobln.Pager
```

## Verifying installation

- **References:** confirm `Gobln.Pager` (and any extension packages you installed) appear in your project references.
- **Version:** the core package is currently at `2.0.0`; the JSON packages are at `1.0.0`.

## Minimal example

```csharp
using Gobln.Pager;

var items = Enumerable.Range(1, 100).Select(i => new { Id = i, Name = $"Item {i}" });

// From IEnumerable<T>
var page = items.ToPage(pageIndex: 1, pageSize: 10);

Console.WriteLine($"Page {page.PageIndex} of {page.TotalPageCount}, {page.TotalItemCount} items total.");
foreach (var item in page)
{
    Console.WriteLine(item.Name);
}
```

With `IQueryable<T>` (e.g. Entity Framework), the same extension method skips/takes at the query level so only the requested page is materialized:

```csharp
using var context = new MyDbContext();

var page = context.MyEntities
    .AsQueryable()
    .ToPage(pageIndex: 1, pageSize: 10);

foreach (var item in page)
{
    Console.WriteLine(item.Name);
}
```

An async equivalent (`ToPageAsync`) is available on both `IEnumerable<T>` and `IQueryable<T>` — see [Extension Methods](Extension-Methods.md).

## Where to go next

- [Core Library](Core-Library.md) for the `Page<T>` / `PagedList<T>` types
- [Extension Methods](Extension-Methods.md) for the full `ToPage`/`ToPagedList` overload list
- [Blazor Integration](Blazor-Integration.md) or [MVC Integration](Mvc-Integration.md) if you need ready-made pagination UI

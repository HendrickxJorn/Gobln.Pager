# Gobln.Pager Wiki

**Gobln.Pager** is a portable, zero-dependency paging library for .NET. It converts collections (`IEnumerable<T>`, `IQueryable<T>`) into strongly-typed paginated results, with optional integrations for Entity Framework Core, Blazor, ASP.NET MVC/Core, and JSON serialization.

## Why Gobln.Pager?

- **Framework agnostic:** targets .NET Framework 4.6.2/4.8, .NET Standard 2.0/2.1, and .NET 8/10
- **LINQ integration:** works on both `IEnumerable<T>` and `IQueryable<T>` (including Entity Framework), synchronously or asynchronously, with EF Core's own async queries through `Gobln.Pager.EntityFrameworkCore`
- **Pre-paged support:** wrap data you've already paged and counted at the database level without re-slicing it
- **Dedicated UI packages:** ready-made Blazor component and ASP.NET MVC/Core HTML helper
- **JSON support:** converters for both `System.Text.Json` and `Newtonsoft.Json`
- **No external dependencies** in the core package

## Pages

| Page | Contents |
| --- | --- |
| [Getting Started](Getting-Started.md) | Package installation, prerequisites, minimal example |
| [Core Library](Core-Library.md) | `Page<T>`, `PagedList<T>`, `PageFilter`, `PageWindow`, interfaces |
| [Extension Methods](Extension-Methods.md) | `ToPage`, `ToPagedList` and their async variants for `IEnumerable<T>` / `IQueryable<T>` |
| [Blazor Integration](Blazor-Integration.md) | The `Pager` Razor component (`Gobln.Pager.Blazor`) |
| [MVC / ASP.NET Core Integration](Mvc-Integration.md) | The `Html.Pager()` helper and `PagerOptions` (`Gobln.Pager.Mvc`) |
| [JSON Serialization](Json-Serialization.md) | `System.Text.Json` and `Newtonsoft.Json` converters |
| [Best Practices & Troubleshooting](Best-Practices-and-Troubleshooting.md) | Recommendations and common pitfalls |

## Package Overview

| Package | Target Frameworks | Purpose |
| --- | --- | --- |
| `Gobln.Pager` | net462, net48, net8.0, net10.0, netstandard2.0, netstandard2.1 | Core paging types and LINQ extension methods |
| `Gobln.Pager.EntityFrameworkCore` | net8.0, net10.0 | Asynchronous `ToPageAsync` / `ToPagedListAsync` for EF Core queries |
| `Gobln.Pager.Blazor` | net8.0, net10.0 | `<Pager>` Razor component |
| `Gobln.Pager.Mvc` | net462, net48, net8.0, net10.0 | `Html.Pager()` helper for ASP.NET MVC / ASP.NET Core |
| `Gobln.Pager.Json.Microsoft` | net462, net48, net8.0, net10.0, netstandard2.0, netstandard2.1 | `System.Text.Json` converters |
| `Gobln.Pager.Json.Newtonsoft` | net462, net48, net8.0, net10.0, netstandard2.0, netstandard2.1 | `Newtonsoft.Json` converters |

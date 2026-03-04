# Gobln.Pager - Complete Documentation

## Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Quick Start](#quick-start)
4. [Installation](#installation)
5. [Architecture](#architecture)
6. [Core Components](#core-components)
7. [Usage Examples](#usage-examples)
8. [API Reference](#api-reference)
9. [Extension Packages](#extension-packages)
10. [Best Practices](#best-practices)
11. [Troubleshooting](#troubleshooting)

---

## Overview

**Gobln.Pager** is a portable, zero-dependency paging library for .NET designed to simplify converting collections (`IEnumerable<T>`, `IQueryable<T>`, and `IList<T>`) into paginated results. The library provides a clean, strongly-typed API for managing pagination across multiple frameworks and data sources.

- **Version:** 2.0.0
- **License:** MIT
- **Author:** Jorn Hendrickx
- **Repository:** [GitHub - HendrickxJorn/Gobln.Pager](https://github.com/HendrickxJorn/Gobln.Pager)

### Why Gobln.Pager?

- **Framework Agnostic:** Works with .NET Framework 4.6.2+, .NET Standard 2.0/2.1, .NET 6, 8, and 10
- **Multiple UI Frameworks:** Dedicated packages for Blazor, ASP.NET MVC, and standard applications
- **JSON Serialization Support:** Built-in integration with both Microsoft System.Text.Json and Newtonsoft.Json
- **LINQ Integration:** Seamless integration with both `IEnumerable<T>` and `IQueryable<T>` (including Entity Framework)
- **Type-Safe:** Strongly-typed generic API prevents runtime type errors
- **No External Dependencies:** Lightweight and self-contained

---

## Features

### Core Functionality
- Convert any `IEnumerable<T>` or `IQueryable<T>` into paginated `Page<T>` objects
- Support for pre-paged datasets (when you've already filtered and counted at the database level)
- Dynamic collection management with `PagedList<T>` (add, remove, navigate between pages)
- Automatic page count calculation and metadata tracking

### Data Types
- **`Page<T>`** - Read-only snapshot of a single page with metadata
- **`PagedList<T>`** - Mutable, navigable collection maintaining paging state
- **`PageFilter`** - DTO for pagination parameters (page index, page size)

### Built-in Validators
- Page index must be >= 1
- Page size must be >= 1
- Total item count must be >= 0

### Multi-Target Support
- **.NET Framework 4.6.2, 4.8**
- **.NET Standard 2.0, 2.1**
- **.NET 6, 8, 10**

---

## Quick Start

### Install via NuGet

Core paging library
dotnet add package Gobln.Pager
For Blazor applications
dotnet add package Gobln.Pager.Blazor
For ASP.NET MVC applications
dotnet add package Gobln.Pager.Mvc
For JSON serialization with System.Text.Json
dotnet add package Gobln.Pager.Json.Microsoft
For JSON serialization with Newtonsoft.Json
dotnet add package Gobln.Pager.Json.Newtonsoft

### Minimal Example

```csharp
using Gobln.Pager;

// Sample data
var items = Enumerable.Range(1, 100).Select(i => new { Id = i, Name = $"Item {i}" });

// Usage with IQueryable (e.g., Entity Framework)
using (var context = new MyDbContext())
{
    var queryable = context.MyEntities.AsQueryable();

    // Get paged result
    var pagedResult = queryable.ToPagedResult(1, 10);

    foreach (var item in pagedResult)
    {
        Console.WriteLine(item.Name);
    }
}

// Usage with IEnumerable
{
    var enumerable = items.AsEnumerable();

    // Get paged result
    var pagedResult = enumerable.ToPagedResult(1, 10);

    foreach (var item in pagedResult)
    {
        Console.WriteLine(item.Name);
    }
}
```

---

## Installation

### Prerequisites

- **.NET Framework 4.6.2+** or **.NET Standard 2.0+** or **.NET 6+**
- Visual Studio 2015+ or any .NET-compatible IDE

### Package Installation Methods

#### Option 1: Package Manager Console (Visual Studio)
```powershell
Install-Package Gobln.Pager
```

#### Option 2: .NET CLI
```bash
dotnet add package Gobln.Pager
```

### Verify Installation

To verify that the Gobln.Pager package is installed correctly, check the following:

- **References:** Ensure `Gobln.Pager` is listed in your project references.
- **Version:** Confirm the installed version matches the expected version (2.0.0 or later).
- **Dependencies:** Check that all required dependencies are also installed (e.g., `Gobln.Pager.Blazor` for Blazor projects).

---

## Architecture

### High-Level Design

---

### Thread Safety

- **`Page<T>`**: Thread-safe after creation (read-only)
- **`PagedList<T>`**: NOT thread-safe; use locking or concurrent collections in multi-threaded scenarios
- ** Extension methods**: Thread-safe (stateless)

---

## Core Components

### 1. `Page<T>` - Single Page Representation

A strongly-typed, read-only snapshot of a single page from a larger collection.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `PageIndex` | `int` | Current page number (1-based indexing) |
| `PageSize` | `int` | Number of items per page |
| `TotalItemCount` | `int` | Total items in original collection |
| `TotalPageCount` | `int` | Total pages in original collection |
| `HasPaging` | `bool` | True if collection spans multiple pages |
| `IsEmpty` | `bool` | True if page contains no items |

#### Constructors

### 2. `PagedList<T>` - Mutable Paginated Collection

A mutable, navigable collection that maintains paging state and allows item manipulation.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `CurrentPageIndex` | `int` | Index of currently active page |
| `PageSize` | `int` | Items per page |
| `PageCount` | `int` | Total number of pages |
| `Count` | `int` | Total items in all pages |
| `IsReadOnly` | `bool` | Always `false` |

#### Key Methods

| Method | Description |
|--------|-------------|
| `Add(T item)` | Add an item to the collection |
| `Remove(T item)` | Remove an item from the collection |
| `Insert(int index, T item)` | Insert an item at the specified index |
| `Clear()` | Remove all items from the collection |
| `ToPagedResult(int pageIndex, int pageSize)` | Get a paged result for the current collection state |

---

## Usage Examples

### 1. Basic Usage

#### Using `Page<T>`

```csharp
using Gobln.Pager;

var items = Enumerable.Range(1, 100).Select(i => new { Id = i, Name = $"Item {i}" });

// Get paged result from IEnumerable
var page = items.ToPagedResult(1, 10);

Console.WriteLine($"Page {page.PageIndex} of {page.TotalPageCount}, {page.TotalItemCount} items total.");
foreach (var item in page)
{
    Console.WriteLine(item.Name);
}
```

#### Using `PagedList<T>`

```csharp
using Gobln.Pager;

var pagedList = new PagedList<int>(1, 10, 50);

// Add items to the list
pagedList.Add(1);
pagedList.Add(2);
pagedList.Add(3);

// Navigate pages
var page = pagedList.ToPagedResult(2, 10);
Console.WriteLine($"Current page: {pagedList.CurrentPageIndex} with {pagedList.Count} total items.");
```

---

## API Reference

- **`Gobln.Pager` Namespace**
  - `Page<T>`
  - `PagedList<T>`
  - `PageFilter`
  - Extension methods for `IEnumerable<T>` and `IQueryable<T>`

---

## Extension Packages

- **Gobln.Pager.Blazor**: Components and helpers for integrating Gobln.Pager with Blazor applications
- **Gobln.Pager.Mvc**: Helpers and filters for ASP.NET MVC integration
- **Gobln.Pager.Json.Microsoft**: JSON serialization support using System.Text.Json
- **Gobln.Pager.Json.Newtonsoft**: JSON serialization support using Newtonsoft.Json

---

## Best Practices

- Prefer `IQueryable<T>` over `IEnumerable<T>` when working with large datasets for better performance
- Always validate pagination parameters (e.g., page index, page size) to avoid runtime errors
- Use `PagedList<T>` for scenarios where the collection needs to be modified after pagination
- Leverage built-in JSON serialization support for efficient data transfer in web applications

---

## Troubleshooting

### Common Issues

- **Problem:** Pagination not working as expected with Entity Framework IQueryable.
  - **Solution:** Ensure the query is executed (e.g., with ToListAsync or similar) before applying pagination extensions.
- **Problem:** Page index or size validation errors.
  - **Solution:** Check that the client application sends valid pagination parameters (>= 1).

### Getting Help

- **Documentation:** Refer to the latest documentation for detailed usage instructions and API references.
- **GitHub Issues:** Check or report issues on the [GitHub repository](https://github.com/HendrickxJorn/Gobln.Pager/issues)
- **Community Forums:** Engage with the community on relevant forums or Stack Overflow for common troubleshooting.

---

## API Reference

- **`Gobln.Pager` Namespace**
  - `Page<T>`
  - `PagedList<T>`
  - `PageFilter`
  - Extension methods for `IEnumerable<T>` and `IQueryable<T>`

---

## Extension Packages

- **Gobln.Pager.Blazor**: Components and helpers for integrating Gobln.Pager with Blazor applications
- **Gobln.Pager.Mvc**: Helpers and filters for ASP.NET MVC integration
- **Gobln.Pager.Json.Microsoft**: JSON serialization support using System.Text.Json
- **Gobln.Pager.Json.Newtonsoft**: JSON serialization support using Newtonsoft.Json

---

## Best Practices

- Prefer `IQueryable<T>` over `IEnumerable<T>` when working with large datasets for better performance
- Always validate pagination parameters (e.g., page index, page size) to avoid runtime errors
- Use `PagedList<T>` for scenarios where the collection needs to be modified after pagination
- Leverage built-in JSON serialization support for efficient data transfer in web applications

---

## Troubleshooting

### Common Issues

- **Problem:** Pagination not working as expected with Entity Framework IQueryable.
  - **Solution:** Ensure the query is executed (e.g., with ToListAsync or similar) before applying pagination extensions.
- **Problem:** Page index or size validation errors.
  - **Solution:** Check that the client application sends valid pagination parameters (>= 1).

### Getting Help

- **Documentation:** Refer to the latest documentation for detailed usage instructions and API references.
- **GitHub Issues:** Check or report issues on the [GitHub repository](https://github.com/HendrickxJorn/Gobln.Pager/issues)
- **Community Forums:** Engage with the community on relevant forums or Stack Overflow for common troubleshooting.

---

## PageDefinition (Internal)

Internal class handling pagination calculations.

#### Calculated Properties

#### Validation Rules

- `pageIndex < 1` → Throws `ArgumentOutOfRangeException`
- `pageSize < 1` → Throws `ArgumentOutOfRangeException`
- `totalItemCount < 0` → Throws `ArgumentOutOfRangeException`

---

## Usage Examples

### Example 1: Simple List Pagination

```csharp
using Gobln.Pager;

// Sample data
var items = Enumerable.Range(1, 100).Select(i => new { Id = i, Name = $"Item {i}" });

// Get paged result from IEnumerable
var page = items.ToPagedResult(1, 10);

Console.WriteLine($"Page {page.PageIndex} of {page.TotalPageCount}, {page.TotalItemCount} items total.");
foreach (var item in page)
{
    Console.WriteLine(item.Name);
}
```

### Example 2: Database Query Pagination (Entity Framework)

```csharp
using Gobln.Pager;
using YourNamespace.Data; // Adjust namespace for your DbContext

// Using a DbContext to fetch data
using (var context = new YourDbContext())
{
    var queryable = context.YourEntities.AsQueryable();

    // Get paged result
    var pagedResult = queryable.ToPagedResult(2, 5);

    Console.WriteLine($"Page {pagedResult.PageIndex} of {pagedResult.TotalPageCount}, {pagedResult.TotalItemCount} items total.");
    foreach (var item in pagedResult)
    {
        Console.WriteLine(item.Name);
    }
}
```

### Example 3: Web API Controller (ASP.NET MVC)

```csharp
using Gobln.Pager;
using YourNamespace.Models; // Adjust namespace for your models
using YourNamespace.Data; // Adjust namespace for your DbContext
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YourEntityController : ControllerBase
    {
        private readonly YourDbContext _context;

        public YourEntityController(YourDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetEntities(int pageIndex = 1, int pageSize = 10)
        {
            var queryable = _context.YourEntities.AsQueryable();

            // Apply pagination
            var pagedResult = queryable.ToPagedResult(pageIndex, pageSize);

            return Ok(pagedResult);
        }
    }
}
```

### Example 4: Pre-Paged Data

```csharp
using Gobln.Pager;

// Sample pre-paged data
var prePagedData = new List<string> { "Item 1", "Item 2", "Item 3" };

// Create a Page directly
var page = new Page<string>(1, 3, 3, prePagedData);

Console.WriteLine($"Pre-paged data: {string.Join(", ", page)}");
```

### Example 5: Using PagedList for Dynamic Collections

```csharp
using Gobln.Pager;

var pagedList = new PagedList<int>(1, 10, 50);

// Add items to the list
pagedList.Add(1);
pagedList.Add(2);
pagedList.Add(3);

// Insert item at specific position
pagedList.Insert(0, 0);

// Remove an item
pagedList.Remove(2);

// Navigate pages
var page = pagedList.ToPagedResult(1, 10);
Console.WriteLine($"Current page after modifications: {pagedList.CurrentPageIndex} with {pagedList.Count} total items.");
```

### Example 6: Razor Page (ASP.NET Core)

```csharp
@page
@model YourNamespace.Pages.YourPageModel
@using Gobln.Pager
@using YourNamespace.Data
@inject YourDbContext context

<h1>Your Items</h1>

@{ 
    // Sample pagination logic
    var pageIndex = 1;
    var pageSize = 10;

    // Fetch paged data
    var pagedResult = context.YourEntities.AsQueryable().ToPagedResult(pageIndex, pageSize);
}

<table>
    <thead>
        <tr>
            <th>ID</th>
            <th>Name</th>
        </tr>
    </thead>
    <tbody>
    @foreach (var item in Model.PagedResult)
    {
        <tr>
            <td>@item.Id</td>
            <td>@item.Name</td>
        </tr>
    }
    </tbody>
</table>

<div>
    Page @(Model.PagedResult.PageIndex) of @(Model.PagedResult.TotalPageCount)
</div>

<nav>
    <ul class="pager">
        <li class="previous @(Model.PagedResult.PageIndex == 1 ? "disabled" : "")">
            <a href="@Url.Page(null, null, new { pageIndex = Model.PagedResult.PageIndex - 1 })">← Older</a>
        </li>
        <li class="next @(Model.PagedResult.PageIndex == Model.PagedResult.TotalPageCount ? "disabled" : "")">
            <a href="@Url.Page(null, null, new { pageIndex = Model.PagedResult.PageIndex + 1 })">Newer →</a>
        </li>
    </ul>
</nav>
```

### Example 7: Blazor Component

```razor
@page "/items"
@using Gobln.Pager
@using YourNamespace.Data
@inject YourDbContext context

<PageTitle>Items</PageTitle>

<h3>Items</h3>

@{
    var pageIndex = 1;
    var pageSize = 10;

    // Fetch paged data
    var pagedResult = new Page<YourEntity>();

    // On parameters set, load data
    protected override void OnParametersSet()
    {
        LoadData();
    }

    // Method to load data
    async Task LoadData()
    {
        pagedResult = await context.YourEntities
            .AsQueryable()
            .ToPagedResultAsync(pageIndex, pageSize);
    }
}

@if (pagedResult != null)
{
    <p>Page @(pagedResult.PageIndex) of @(pagedResult.TotalPageCount)</p>

    <table class="table">
        <thead>
            <tr>
                <th>ID</th>
                <th>Name</th>
            </tr>
        </thead>
        <tbody>
        @foreach (var item in pagedResult)
        {
            <tr>
                <td>@item.Id</td>
                <td>@item.Name</td>
            </tr>
        }
        </tbody>
    </table>

    <div>
        <button @onclick="() => { pagedResult.GoToPreviousPage(); LoadData(); }" 
                disabled="@(pagedResult.PageIndex == 1)">Previous</button>
        <button @onclick="() => { pagedResult.GoToNextPage(); LoadData(); }" 
                disabled="@(pagedResult.PageIndex == pagedResult.TotalPageCount)">Next</button>
    </div>
}
else
{
    <p><em>Loading...</em></p>
}
```

---

## API Reference

### `IEnumerableExtensions`

Extension methods for `IEnumerable<T>`.

#### Methods

### `IQueryableExtensions`

Extension methods for `IQueryable<T>` (Entity Framework, LINQ-to-SQL, etc.).

#### Methods
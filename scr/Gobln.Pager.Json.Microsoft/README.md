# Gobln.Pager.Json.Microsoft

[![NuGet](https://img.shields.io/nuget/dt/Gobln.Pager.Json.Microsoft.svg)](https://www.nuget.org/packages/Gobln.Pager.Json.Microsoft/)
[![NuGet](https://img.shields.io/nuget/v/Gobln.Pager.Json.Microsoft.svg)](https://www.nuget.org/packages/Gobln.Pager.Json.Microsoft/)

`System.Text.Json` converters for [Gobln.Pager](https://www.nuget.org/packages/Gobln.Pager/). Without them a `Page<T>` is written as a plain array of its items, and its page index, page size and total item count are lost. With them a page arrives the way it was sent.

## Frameworks

- .NET 8.0 and .NET 10.0
- .NET Standard 2.0 and 2.1
- .NET Framework 4.6.2 and 4.8

## Installation

```shell
dotnet add package Gobln.Pager.Json.Microsoft
```

## Usage

Add the converters to the serializer options:

```csharp
using Gobln.Pager;
using Gobln.Pager.Json.Microsoft;
using System.Text.Json;

var options = new JsonSerializerOptions();
options.Converters.Add(new PageJsonConverter());
options.Converters.Add(new PagedListJsonConverter());

var json = JsonSerializer.Serialize(page, options);

var samePage = JsonSerializer.Deserialize<Page<Order>>(json, options);
```

In ASP.NET Core, add them once in `Program.cs`. Every action that returns a page then uses them:

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new PageJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new PagedListJsonConverter());
    });
```

The converters work for every item type, and for the interfaces as well: `PageJsonConverter` handles `Page<T>` and `IPage<T>`, `PagedListJsonConverter` handles `PagedList<T>` and `IPagedList<T>`.

## JSON format

A `Page<T>` is written as an object: its items, and what is needed to build the page again.

```json
{"PageItems":[1,2,3],"PageIndex":2,"PageSize":3,"TotalItemCount":42}
```

The property names follow the naming policy of the serializer options. With ASP.NET Core's defaults they are `pageItems`, `pageIndex`, `pageSize` and `totalItemCount`.

A `PagedList<T>` is written as a plain array of all its items:

```json
[1,2,3,4,5]
```

Its page size and current page are not written. A list that is read back has a page size of 10 and is on page 1.

[Gobln.Pager.Json.Newtonsoft](https://www.nuget.org/packages/Gobln.Pager.Json.Newtonsoft/) writes the same format, so JSON written with one package can be read with the other.

## License

[MIT](https://github.com/HendrickxJorn/Gobln.Pager/blob/master/LICENSE.md)

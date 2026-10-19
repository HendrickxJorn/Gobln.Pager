# JSON Serialization

Two separate packages provide converters, mirroring each other's feature set for `System.Text.Json` and `Newtonsoft.Json` respectively. Both project-reference the core `Gobln.Pager` package and target the same framework matrix as core (net462, net48, net8.0, net10.0, netstandard2.0, netstandard2.1).

Each package ships two converters:

- `PageJsonConverter` — for `Page<T>` / `IPage<T>`
- `PagedListJsonConverter` — for `PagedList<T>` / `IPagedList<T>`

Without them, both serializers write a `Page<T>` as a plain array of its items, and its page index, page size and total item count are lost.

## `Gobln.Pager.Json.Microsoft` (`System.Text.Json`)

```csharp
using System.Text.Json;
using Gobln.Pager.Json.Microsoft;

var options = new JsonSerializerOptions();
options.Converters.Add(new PageJsonConverter());
options.Converters.Add(new PagedListJsonConverter());

var json = JsonSerializer.Serialize(myPage, options);
var page = JsonSerializer.Deserialize<Page<MyItem>>(json, options);
```

Both are `JsonConverterFactory` implementations that resolve to an internal generic converter per element type via reflection, so a single registered instance handles `Page<T>`/`IPage<T>` (or `PagedList<T>`/`IPagedList<T>`) for any `T`.

In ASP.NET Core, register them once in `Program.cs`:

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new PageJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new PagedListJsonConverter());
    });
```

**`PagedList<T>` wire format** — a plain JSON array of items:

```json
[ { "Id": 1, "Name": "Item 1" }, { "Id": 2, "Name": "Item 2" } ]
```

**`Page<T>` wire format** — an object carrying the page metadata and its items:

```json
{
  "PageItems": [ { "Id": 1, "Name": "Item 1" } ],
  "PageIndex": 1,
  "PageSize": 10,
  "TotalItemCount": 42
}
```

The property names follow the serializer's naming policy. With ASP.NET Core's defaults they are `pageItems`, `pageIndex`, `pageSize` and `totalItemCount`.

## `Gobln.Pager.Json.Newtonsoft` (`Newtonsoft.Json`)

```csharp
using Newtonsoft.Json;
using Gobln.Pager.Json.Newtonsoft;

var settings = new JsonSerializerSettings
{
    Converters = { new PageJsonConverter(), new PagedListJsonConverter() }
};

var json = JsonConvert.SerializeObject(myPage, settings);
var page = JsonConvert.DeserializeObject<Page<MyItem>>(json, settings);
```

In ASP.NET Core with `Microsoft.AspNetCore.Mvc.NewtonsoftJson`:

```csharp
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new PageJsonConverter());
        options.SerializerSettings.Converters.Add(new PagedListJsonConverter());
    });
```

The property names follow the contract resolver of the settings, so with ASP.NET Core's defaults they are camel-cased here too.

**Wire formats are identical to the `System.Text.Json` package** — a plain array of items for `PagedList<T>`, and the same metadata object for `Page<T>`. JSON written by either package is readable by the other, so a `System.Text.Json`-based API and a `Newtonsoft.Json`-based client interoperate directly.

> Both packages assert this shared format against the same literal JSON in their own test suites (`JsonWireFormatTest`), so the two cannot drift apart unnoticed.

Both `Page<T>` converters reconstruct via `new Page<T>(items, pageIndex, pageSize, totalItemCount)`. A page without items keeps its totals too, so a pre-paged page that came back empty still tells a pager how many pages there are. Only an empty page with a `TotalItemCount` of `0`, which is what `new Page<T>()` writes, is read back as `new Page<T>()`. Both `PagedList<T>` converters reconstruct via `new PagedList<T>(items)`: the page size and current page are not written, so a list that is read back has page size 10 and is on page 1.

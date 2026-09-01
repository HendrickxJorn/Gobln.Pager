# JSON Serialization

Two separate packages provide converters, mirroring each other's feature set for `System.Text.Json` and `Newtonsoft.Json` respectively. Both project-reference the core `Gobln.Pager` package and target the same framework matrix as core (net462, net48, net8.0, net10.0, netstandard2.0, netstandard2.1).

Each package ships two converters:

- `PageJsonConverter` — for `Page<T>` / `IPage<T>`
- `PagedListJsonConverter` — for `PagedList<T>` / `IPagedList<T>`

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

**Wire formats are identical to the `System.Text.Json` package** — a plain array of items for `PagedList<T>`, and the same metadata object for `Page<T>`. JSON written by either package is readable by the other, so a `System.Text.Json`-based API and a `Newtonsoft.Json`-based client interoperate directly.

> Both packages assert this shared format against the same literal JSON in their own test suites (`JsonWireFormatTest`), so the two cannot drift apart unnoticed.

Both `Page<T>` converters reconstruct via `new Page<T>(items, pageIndex, pageSize, totalItemCount)`, and treat an empty/missing `PageItems` array as an empty `Page<T>`. Both `PagedList<T>` converters reconstruct via `new PagedList<T>(items)` (default page size 10).

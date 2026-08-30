# Code review — Gobln.Pager (branch `jorn/V2`)

**Date:** 2026-08-13
**Scope:** the uncommitted V2 changes (working tree vs. `master`) across `Gobln.Pager`, `Gobln.Pager.Mvc`, `Gobln.Pager.Blazor`, `Gobln.Pager.Json.Microsoft`, `Gobln.Pager.Json.Newtonsoft` and `Gobln.Pager.BlazorTest`.
**Verification:** every finding below was reproduced against the built libraries, not only read from the diff.
**Test baseline at review time:** `Gobln.Pager.MvcTest` 21/21 pass, `Gobln.Pager.BlazorTest` 3/3 pass — **none** of the findings below is caught by the current suite.

---

## Summary

| ID | Severity | Area | Location | Problem |
|----|----------|------|----------|---------|
| [B01](#b01) | Critical | Core | `PageDefinition.cs:7-30` | Properties are order-dependent; `SkipIndex`/`PageCount` use raw ctor args, `PageCount` can divide by zero |
| [B02](#b02) | Critical | Core | `Page.cs:109-120` | Page metadata is only assigned when the slice is non-empty (regression vs. `master`) |
| [B03](#b03) | High | Core | `PagedList.cs:39-46` | `PagedList(int pageSize)` leaves `PageCount = 0`; `GetPage()` then throws |
| [B04](#b04) | High | MVC | `PagerFactory.cs:19,40-42` | `Render` throws `NullReferenceException` on a null page and when `SetOptions` was never called |
| [B05](#b05) | High | MVC | `HtmlExtension.cs:72-74` | A relative `HRefUrl` now reaches `UriBuilder` → `UriFormatException` |
| [B06](#b06) | High | MVC | `HtmlExtension.cs:62-76` | The resolved request URL is written back into the caller's / the static default options |
| [B07](#b07) | High | JSON | `Json.Microsoft/PageJsonConverter.cs:43,64,91` | Microsoft writes `[{…}]`, Newtonsoft writes `{…}` — the two packages cannot read each other |
| [B08](#b08) | High | JSON | `Json.Newtonsoft/PageJsonConverter.cs:66` | `ReadJson` ignores the `JsonSerializer` settings that `WriteJson` now honours |
| [B09](#b09) | Medium | MVC | `PagerFactory.cs:82-83` | The Next button tests `ShowPreviousButtonLink`; `ShowNextButtonLink` is never read |
| [B10](#b10) | Medium | MVC | `PagerFactory.cs:430-436` | Boundary page links are rendered twice (the `Pager.razor` fix was not mirrored here) |
| [B11](#b11) | Medium | Core | `Page.cs:113` | `_items` keeps the deferred `Skip/Take` query instead of materializing it |
| [B12](#b12) | Medium | Blazor | `Pager.razor.cs:69-79` | `_lastSeenPageIndex` cannot tell "unrelated re-render" from "parent rejected the change" |
| [B13](#b13) | Medium | MVC | `PagerFactory.cs:228` | `ShowIndexAsRange` upper bound is not clamped to `TotalItemCount` |
| [B14](#b14) | Medium | Tests | `PagerComponentTest02.cs:17-18,82` | The new test stole the existing `[Fact]`; the old test no longer runs |
| [B15](#b15) | Medium | MVC | `PagerServiceCollectionExcetntions.cs:17-24` | `AddMvcPager` hard-casts `IPagerOptions` and registers nothing in DI |

Plus [minor cleanups](#minor-cleanups) M01–M07 and [suggested regression tests](#suggested-regression-tests).

**Read B01 → B02 → B11 as one chain.** They share a root cause and are best fixed together in that order.

---

<a id="b01"></a>
## B01 — `PageDefinition` properties are order-dependent (Critical)

**Where:** [scr/Gobln.Pager/Infrastructure/PageDefinition.cs:7-30](../scr/Gobln.Pager/Infrastructure/PageDefinition.cs#L7-L30)

**What's wrong:** `PageSize` and `PageIndex` sanitize the primary-constructor parameters, but `SkipIndex` and `PageCount` read the **raw** `pageIndex` / `pageSize`. Worse, the `PageIndex` getter *writes* to the captured `pageIndex` parameter, so reading one property changes what another property returns.

**How it fails:**

- `PrivateToPage` evaluates `pd.SkipIndex` before anything reads `pd.PageIndex`, so the clamp never reaches the skip. `new PageDefinition(99, 10, 25)` → `SkipIndex = 980` (`Skip(980)` returns an empty slice) while `PageIndex` reports the clamped `3`. This is the direct cause of [B02](#b02).
- Reading `pd.PageIndex` *first* rewrites `pageIndex` to `1`, after which `SkipIndex` returns `0`. The same object answers differently depending on access order.
- `PageCount` divides by the raw `pageSize`, not by the sanitized `PageSize` property (which maps `<= 0` to `int.MaxValue`). Any `pageSize == 0` with `totalItemCount > 0` is a `DivideByZeroException` instead of the single-page result `PageSize` clearly intends.

**Possible solution:** compute all values once in a real constructor and expose them as immutable properties. Derive `SkipIndex` from the *sanitized* `PageIndex`/`PageSize`, which also removes the `(pageIndex - 1) * pageSize` overflow risk (after clamping, `(PageIndex - 1) * PageSize < TotalItemCount`).

```csharp
internal sealed class PageDefinition
{
    public PageDefinition(int pageIndex, int pageSize, int totalItemCount)
    {
        TotalItemCount = totalItemCount < 0 ? 0 : totalItemCount;

        PageSize = TotalItemCount == 0
            ? 0
            : (pageSize <= 0 ? int.MaxValue : pageSize);

        PageCount = TotalItemCount == 0 || PageSize == 0
            ? 1
            : (int)Math.Ceiling(TotalItemCount / (decimal)PageSize);

        PageIndex = TotalItemCount == 0
            ? 1
            : Math.Clamp(pageIndex, 1, PageCount);

        SkipIndex = TotalItemCount == 0
            ? 0
            : (PageIndex - 1) * PageSize;
    }

    public int PageSize { get; }

    public int PageIndex { get; }

    public int PageCount { get; }

    public int SkipIndex { get; }

    public int TotalItemCount { get; }
}
```

**Note:** this changes behaviour for an out-of-range `pageIndex` from "empty slice" to "last page", which is what `PageIndex` already claims. Decide explicitly whether an out-of-range page should clamp to the last page (the fix above) or throw — but it should not silently return an empty page while reporting a valid index.

---

<a id="b02"></a>
## B02 — Page metadata is dropped when the slice is empty (Critical, regression)

**Where:** [scr/Gobln.Pager/Page.cs:109-120](../scr/Gobln.Pager/Page.cs#L109-L120)

**What's wrong:** `FillPageData` assigns `PageIndex`, `PageSize`, `TotalItemCount` and `TotalPageCount` **inside** the `collection.Any()` guard. The metadata does not depend on the items, so an empty slice silently yields the default metadata. `master` always assigned it.

**How it fails (reproduced):**

| Call | Actual | Expected (`master`) |
|------|--------|---------------------|
| `Enumerable.Range(1, 25).ToList().ToPage(5, 10)` | `PageIndex=1 PageSize=0 TotalItemCount=0 TotalPageCount=1 HasPaging=False` | `PageIndex=3 PageSize=10 TotalItemCount=25 TotalPageCount=3` |
| `Enumerable.Range(1, 25).ToList().ToPage(99, 10)` | same | same |
| `new List<int>().ToPage(3, 10, 25)` (pre-paged) | `TotalItemCount=0` | `TotalItemCount=25` |

Because `HasPaging` flips to `false`, any pager bound to that `Page` disappears from the UI entirely, and the caller loses the item count.

**Possible solution:** always assign the metadata; only the items are conditional. (`collection.Any()` also causes an extra enumeration — see [B11](#b11).)

```csharp
private void FillPageData(IEnumerable<T> collection, PageDefinition pageDefinitions)
{
    _items = collection is null ? [] : [.. collection];

    PageIndex = pageDefinitions.PageIndex;
    TotalPageCount = pageDefinitions.PageCount;
    PageSize = pageDefinitions.PageSize;
    TotalItemCount = pageDefinitions.TotalItemCount;
}
```

For an empty source this still produces `PageIndex=1, PageSize=0, TotalItemCount=0, TotalPageCount=1` (identical to today's defaults), because `PageDefinition` maps `totalItemCount == 0` to exactly those values — so nothing regresses for the empty case.

Fix [B01](#b01) at the same time, otherwise `ToPage(5, 10)` will report `PageIndex=3` while still returning an empty item set.

---

<a id="b03"></a>
## B03 — `new PagedList<T>()` has `PageCount = 0` and `GetPage()` throws (High)

**Where:** [scr/Gobln.Pager/PagedList.cs:39-46](../scr/Gobln.Pager/PagedList.cs#L39-L46)

**What's wrong:** the `PagedList(int pageSize)` constructor never calls `RecalculatePageCount()`, so `PageCount` stays at its `int` default of `0`. Only the collection constructor calls it (line 69).

**How it fails (reproduced):**

- `new PagedList<int>()` → `PageCount = 0`, while `new PagedList<int>(new List<int>())` → `PageCount = 1`.
- `PageIndex = 1` stores `0`, because the setter does `else if (PageCount < value) value = PageCount`.
- `GetPage()` then throws `ArgumentOutOfRangeException: Can not be less then zero or zero (Parameter 'pageIndex')`.
- Reachable from both JSON packages, which return `new PagedList<T>()` for an empty array.

**Possible solution (minimal):**

```csharp
public PagedList(int pageSize)
{
    Validator.ValidatePageSize(pageSize);

    _currentPageIndex = 1;
    _pageSize = pageSize;
    _list = [];

    RecalculatePageCount();
}
```

**Possible solution (recommended):** remove the class of bug instead of this instance of it — make `PageCount` computed and clamp in the `PageIndex` getter. Every mutator (`Add`, `Clear`, `Remove`, `Insert`, `RemoveAt`, `AddRange`, `PageSize` setter) currently has to remember to call `RecalculatePageCount()`; with the version below, none of them do, and a stale `_currentPageIndex` after a removal can no longer point past the end.

```csharp
public int PageCount => Count == 0 ? 1 : ((Count - 1) / PageSize) + 1;

public int PageIndex
{
    get => Math.Clamp(_currentPageIndex, 1, PageCount);
    set => _currentPageIndex = Math.Clamp(value, 1, PageCount);
}
```

---

<a id="b04"></a>
## B04 — `PagerFactory.Render` throws `NullReferenceException` (High)

**Where:** [scr/Gobln.Pager.Mvc/PagerFactory.cs:19](../scr/Gobln.Pager.Mvc/PagerFactory.cs#L19) and [:40-42](../scr/Gobln.Pager.Mvc/PagerFactory.cs#L40-L42)

**What's wrong:** two separate null dereferences.

1. The `page == null` guard is short-circuited: `PagerOptions.ShowPager != Always && (page == null || …)`. When `ShowPager == Always` the left operand is `false`, the `page == null` test is never evaluated, and `InternalPage` is set to `null`.
2. `PagerOptions` is never initialized (compiler already says so: `PagerFactory.cs(19,26): warning CS8618`). The public `html.Pager(page, IPagerFactory)` overload ([HtmlExtension.cs:96](../scr/Gobln.Pager.Mvc/HtmlExtension.cs#L96)) calls `Render` **without** `SetOptions`.

**How it fails (reproduced):**

```csharp
factory.SetOptions(new PagerOptions { ShowPager = WhenToShowEnum.Always });
factory.Render(null);              // NullReferenceException

new PagerFactory().Render(page);   // NullReferenceException at PagerOptions.ShowPager
                                   // == html.Pager(page, new PagerFactory())
```

**Possible solution:** give `PagerOptions` a default and split the guard so the null check runs first.

```csharp
public IPagerOptions PagerOptions { get; internal set; } = new PagerOptions();
```

```csharp
public virtual IHtmlContent Render(IPage page)
{
    if (page is null
        || PagerOptions.ShowPager == WhenToShowEnum.Never
        || (PagerOptions.ShowPager == WhenToShowEnum.IfNecessary && !page.HasPaging))
    {
#if NETFRAMEWORK
        return MvcHtmlString.Create(string.Empty);
#else
        return new HtmlContentBuilder();
#endif
    }

    InternalPage = page;
    ...
}
```

This is behaviour-equivalent for `Always` and `IfNecessary` with a non-null page, and it reads as the rule it implements. Prefer `new PagerOptions()` over `PagerSettings.DefaultPagerOptions` as the default so a factory never picks up shared mutable state — see [B06](#b06).

---

<a id="b05"></a>
## B05 — a relative `HRefUrl` throws `UriFormatException` (High, regression)

**Where:** [scr/Gobln.Pager.Mvc/HtmlExtension.cs:72-74](../scr/Gobln.Pager.Mvc/HtmlExtension.cs#L72-L74)

**What's wrong:** the non-absolute fallback was changed from `null` to the original string, so relative URLs now flow into `UrlHelper.AddQueryValuesToUrlString`, which builds a `UriBuilder` and returns `uriBuilder.Uri.AbsoluteUri` — neither of which can represent a relative URL.

**How it fails (reproduced):**

- `HRefUrl = "/products"` → `new UriBuilder("/products")` throws `UriFormatException: Invalid URI: The hostname could not be parsed`, killing the whole view render.
- `HRefUrl = "products?a=1"` does not throw but silently becomes `http://products/?a=1` — the first path segment is reinterpreted as the host.

**Possible solution (minimal, restores `master` behaviour):**

```csharp
pagerOptions.HRefUrl = Uri.TryCreate(pagerOptions.HRefUrl, UriKind.Absolute, out Uri? uriResult)
    ? uriResult.ToString()
    : null;
```

**Possible solution (recommended):** a relative `href` is the normal thing to want in an MVC view, so support it in [UrlHelper.AddQueryValuesToUrlString](../scr/Gobln.Pager.Mvc/Infrastructure/UrlHelper.cs#L58-L86) instead of discarding it — resolve against a throw-away base and return only the relative part:

```csharp
public static string AddQueryValuesToUrlString(string url, Dictionary<string, string> queryValues)
{
    var isAbsolute = Uri.TryCreate(url, UriKind.Absolute, out var absolute);

    var uriBuilder = isAbsolute
        ? new UriBuilder(absolute!)
        : new UriBuilder(new Uri(new Uri("http://relative.invalid"), url));

    // …existing query merge, unchanged…

    return isAbsolute
        ? uriBuilder.Uri.AbsoluteUri
        : uriBuilder.Uri.PathAndQuery;
}
```

Whichever route you take, decide it explicitly: today the code *looks* like it supports relative URLs and does not.

---

<a id="b06"></a>
## B06 — the resolved request URL is written back into shared options (High)

**Where:** [scr/Gobln.Pager.Mvc/HtmlExtension.cs:62-76](../scr/Gobln.Pager.Mvc/HtmlExtension.cs#L62-L76)

**What's wrong:** `Pager<T>` assigns to `pagerOptions.HRefUrl`, i.e. it mutates the object the caller passed in. For the parameterless overload that object is the process-wide static `PagerSettings.DefaultPagerOptions` ([HtmlExtension.cs:37](../scr/Gobln.Pager.Mvc/HtmlExtension.cs#L37)).

**How it fails:** the first `html.Pager(page)` of the process writes e.g. `https://site/products?pageIndex=2` into the singleton. On every later request `HRefUrl` is no longer null/whitespace, so the `string.IsNullOrWhiteSpace` branch is skipped and **every pager in the app emits links pointing at that first request's URL**. Two concurrent requests also race on the same field. [B15](#b15) makes it worse by storing the *caller's own* instance as that singleton.

**Possible solution:** never mutate the options that were handed to you. `PagerOptions` already has a public `Clone()` ([PagerOptions.cs:148](../scr/Gobln.Pager.Mvc/PagerOptions.cs#L148)) — lift it onto the interface and clone before resolving:

```csharp
// Contracts/IPagerOptions.cs
IPagerOptions Clone();
```

```csharp
public IHtmlContent Pager<T>(IPage page, T pagerOptions, IPagerFactory<T> pagerFactory)
    where T : IPagerOptions
{
    ArgumentNullException.ThrowIfNull(pagerOptions);

    var options = (T)pagerOptions.Clone();   // per-render copy

    if (options.HRefUrl is not "#")
    {
        options.HRefUrl = string.IsNullOrWhiteSpace(options.HRefUrl)
            ? (Infrastructure.UrlHelper.HasCurrentUrlString(html)
                ? Infrastructure.UrlHelper.GetCurrentUrlString(html)
                : "#")
            : (Uri.TryCreate(options.HRefUrl, UriKind.Absolute, out var uri) ? uri.ToString() : null);
    }

    pagerFactory.SetOptions(options);

    return Pager(html, page, pagerFactory);
}
```

If you would rather not touch the interface, keep the resolved URL out of the options entirely: pass it to the factory separately (e.g. an internal `HRefUrlOverride` that `UrlCreator` prefers over `PagerOptions.HRefUrl`).

---

<a id="b07"></a>
## B07 — the two JSON packages emit incompatible formats (High)

**Where:** [scr/Gobln.Pager.Json.Microsoft/PageJsonConverter.cs:43](../scr/Gobln.Pager.Json.Microsoft/PageJsonConverter.cs#L43), [:64](../scr/Gobln.Pager.Json.Microsoft/PageJsonConverter.cs#L64), [:91](../scr/Gobln.Pager.Json.Microsoft/PageJsonConverter.cs#L91)

**What's wrong:** the System.Text.Json converter wraps a single `Page` in a JSON **array** (`[{…}]`), while the Newtonsoft converter for the same type writes a bare **object** (`{…}`).

**How it fails:** a service that serializes with `Gobln.Pager.Json.Newtonsoft` produces `{"PageItems":[…],"PageIndex":2,…}`; a client deserializing with `Gobln.Pager.Json.Microsoft` hits `if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException()` and fails. In the other direction, `[{…}]` reaches Newtonsoft's `jsonObj.ToObject<TempPage<T>>()`, which cannot bind a `JArray` to `TempPage`. The array wrapper is also wrong on its own terms — a page is one object, not a one-element array.

**Possible solution:** drop the array wrapper on the Microsoft side (the object form is the correct one, and the one Newtonsoft already produces).

```csharp
public override IPage<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
{
    var temp = JsonSerializer.Deserialize<TempPage<T>>(ref reader, options);

    return temp is null || temp.PageSize < 1
        ? new Page<T>()
        : new Page<T>(temp.PageItems ?? [], temp.PageIndex, temp.PageSize, temp.TotalItemCount);
}

public override void Write(Utf8JsonWriter writer, IPage<T> value, JsonSerializerOptions options)
{
    var tempPage = value is null
        ? new TempPage<T> { PageIndex = 1, PageSize = 0, TotalItemCount = 0, PageItems = null }
        : new TempPage<T>
        {
            PageIndex = value.PageIndex,
            PageSize = value.PageSize,
            TotalItemCount = value.TotalItemCount,
            PageItems = [.. value]
        };

    JsonSerializer.Serialize(writer, tempPage, options);
}
```

Two things worth deciding while you are in here:

- This is a **breaking wire-format change** for anyone already consuming the array form. V2 is the moment to do it, but it belongs in the release notes.
- Falling back on `PageSize < 1` rather than on "no items" preserves the metadata of an empty-but-paged page. (`Validator.Validate` rejects `pageSize < 1`, which is why the `new Page<T>()` fallback exists at all.) Apply the same guard to the Newtonsoft converter — after [B02](#b02) an empty page can legitimately carry a real `TotalItemCount`.

---

<a id="b08"></a>
## B08 — `ReadJson` ignores the serializer settings that `WriteJson` honours (High)

**Where:** [scr/Gobln.Pager.Json.Newtonsoft/PageJsonConverter.cs:66](../scr/Gobln.Pager.Json.Newtonsoft/PageJsonConverter.cs#L66) and [PagedListJsonConverter.cs:66](../scr/Gobln.Pager.Json.Newtonsoft/PagedListJsonConverter.cs#L66)

**What's wrong:** `WriteJson` was changed to use the active serializer (`serializer.Serialize(writer, tempPage)`), but `ReadJson` still calls `jsonObj.ToObject<TempPage<T>>()` **without** the serializer, so reads use Newtonsoft's defaults while writes use the caller's settings.

**How it fails:** with a `CamelCasePropertyNamesContractResolver` (or any custom naming strategy / converter), `WriteJson` emits `{"pageItems":[…],"pageIndex":2,"pageSize":10,"totalItemCount":25}`, and `ReadJson` matches none of those names. `PageItems` comes back `null`, the method returns `new Page<T>()`, and the whole page — items *and* metadata — is silently dropped on round-trip.

**Possible solution:** pass the serializer through in both converters.

```csharp
var tempPage = jsonObj.ToObject<TempPage<T>>(serializer);          // PageJsonConverter
var tempPage = jsonObj.ToObject<IEnumerable<T>>(serializer);       // PagedListJsonConverter
```

---

<a id="b09"></a>
## B09 — the Next button reads `ShowPreviousButtonLink` (Medium)

**Where:** [scr/Gobln.Pager.Mvc/PagerFactory.cs:82-83](../scr/Gobln.Pager.Mvc/PagerFactory.cs#L82-L83)

**What's wrong:** copy-paste from the Previous block above. `ShowNextButtonLink` is never read anywhere in the library.

**How it fails (reproduced):** with `ShowPreviousButtonLink = Always, ShowNextButtonLink = Never` the output still contains `<a class="page-link" href="#">Next</a>`; with the two swapped, the Next link is absent. The option is inert and Next is silently slaved to Previous. The existing tests miss it because [Bootstrap5Test.cs:185-186](../test/Gobln.Pager.MvcTest/Bootstrap5Test.cs#L185-L186) always sets both to `Never` together.

**Possible solution:**

```csharp
if (PagerOptions.ShowNextButtonLink == WhenToShowEnum.Always
    || (PagerOptions.ShowNextButtonLink == WhenToShowEnum.IfNecessary && InternalPage.PageIndex < InternalPage.TotalPageCount))
{
    tagBuilders.Add(CreateNextTag());
}
```

---

<a id="b10"></a>
## B10 — boundary page links are rendered twice in the MVC pager (Medium)

**Where:** [scr/Gobln.Pager.Mvc/PagerFactory.cs:430-436](../scr/Gobln.Pager.Mvc/PagerFactory.cs#L430-L436)

**What's wrong:** the trailing-boundary loop is not clamped against the leading-boundary range. This is the **same defect that was fixed in [Pager.razor:32](../scr/Gobln.Pager.Blazor/Pager.razor#L32) in this diff** and left unfixed in the MVC copy of the algorithm.

**How it fails (reproduced against `PageNumbersCreator`):**

| `TotalPageCount` / `BoundaryCount` | Rendered page links | Expected |
|---|---|---|
| 1 / 1 | `1, 1` | `1` |
| 2 / 2 | `1, 2, 1, 2` | `1, 2` |
| 3 / 2 | `1, 2, 2, 3` | `1, 2, 3` |

Duplicate `<li>` entries also duplicate the `aria-current="page"` marker.

**Possible solution:** mirror the razor guard (`firstIndex` here is the razor's `StartBoundery`):

```csharp
for (int index = Math.Max(InternalPage.TotalPageCount - PagerOptions.BoundaryCount + 1, firstIndex + 1);
     index <= InternalPage.TotalPageCount;
     index++)
{
    tagBuilderList.Add(PrivatePageNumberCreator(index));
}
```

**Improvement worth considering:** the page-number sequence is now implemented twice — once in `Pager.razor` / `Pager.razor.cs` and once in `PagerFactory.PageNumbersCreator`, with the same boundary/sibling/ellipsis arithmetic. They have already drifted apart once (this bug). Extracting the sequence into the core `Gobln.Pager` package (something like `IEnumerable<PageLinkItem> BuildPageLinks(int pageIndex, int totalPageCount, int siblingCount, int boundaryCount, bool showEllipsis)`) would let both renderers consume one tested implementation and would make this class of divergence impossible. It would also become directly unit-testable without a rendering host.

---

<a id="b11"></a>
## B11 — `Page` stores the deferred query instead of materializing it (Medium)

**Where:** [scr/Gobln.Pager/Page.cs:113](../scr/Gobln.Pager/Page.cs#L113) (`_items = collection.AsEnumerable();`; `master` used `ToArray()`)

**What's wrong:** `AsEnumerable()` does not materialize. `_items` holds the un-executed `Skip(...).Take(...)` query, so the source is re-enumerated on every property read and every iteration.

**How it fails (reproduced with a counting iterator):** `source.ToPage(2, 10)` enumerates the source **twice during construction** (`source.Count()` in the extension, plus `collection.Any()` inside `FillPageData`), a third time on reading `.IsEmpty`, and once more per `foreach` — 5 enumerations in the probe. Consequences:

- For an `IQueryable` that is 5 separate database round trips per page.
- Results can differ between two reads of the same `Page`.
- Enumerating the `Page` after the `DbContext` is disposed throws `ObjectDisposedException`.

**Possible solution:** materialize once in `FillPageData` — this is already part of the [B02](#b02) fix:

```csharp
_items = collection is null ? [] : [.. collection];
```

Two follow-ups that come with it:

- With `_items` materialized, `IsEmpty` becomes free and the `collection.Any()` call disappears, removing one of the two construction-time enumerations.
- For the `IQueryable` async overloads, materializing inside the constructor executes the query **synchronously**. If `ToPageAsync` on `IQueryable` is meant to be genuinely async, the slice should be materialized in the extension method (`await ToListAsync(...)` at the call site, EF Core dependency permitting) and handed to `Page` pre-paged — worth checking [IQueryableExtensions.cs](../scr/Gobln.Pager/Extensions/IQueryableExtensions.cs) against that intent.

---

<a id="b12"></a>
## B12 — the Blazor pager gets stuck when the parent does not honour a page change (Medium)

**Where:** [scr/Gobln.Pager.Blazor/Pager.razor.cs:69-79](../scr/Gobln.Pager.Blazor/Pager.razor.cs#L69-L79)

**What's wrong:** `_lastSeenPageIndex` is meant to distinguish "the parent deliberately changed `PageIndex`" from "unrelated re-render", but `InvokePageChange` updates the local `PageIndex` **without** updating `_lastSeenPageIndex`. The heuristic therefore cannot tell an unrelated re-render from the parent authoritatively re-asserting the value we last saw.

**How it fails (reproduced by driving `OnParametersSet`/`InvokePageChange`):**

1. Parent renders with `Page.PageIndex = 1` → `_lastSeen = 1`.
2. User clicks 3 → local `PageIndex = 3`, `_lastSeen` stays `1`.
3. Parent declines the change and re-renders, still at `Page.PageIndex = 1`.
4. `1 == _lastSeen`, so the `else` branch keeps local `3` → the pager highlights page 3 while the parent shows page 1's data.

Second confirmed variant: after the parent accepts page 3, a click on 5 that the parent does not honour leaves the component stuck on 5 forever.

**Possible solution:** make the component **controlled** — `Page.PageIndex` is the single source of truth, and the optimistic local value survives only until the next parent render:

```csharp
protected override void OnParametersSet()
{
    base.OnParametersSet();

    ArgumentNullException.ThrowIfNull(Page, nameof(Page));

    if (SiblingCount < 0) SiblingCount = 0;
    if (BoundaryCount < 0) BoundaryCount = 0;

    TotalPageCount = Page.TotalPageCount < 1 ? 1 : Page.TotalPageCount;
    PageIndex = Math.Clamp(Page.PageIndex, 1, TotalPageCount);
}
```

`_lastSeenPageIndex` then disappears entirely. `InvokePageChange` can keep updating `PageIndex` optimistically for instant feedback; the next parent render simply corrects it, which is exactly what both failure modes need. If a genuinely *uncontrolled* mode is wanted (pager tracks its own index, parent only listens), make that an explicit opt-in parameter rather than an inferred one — the ambiguity is what causes this bug.

---

<a id="b13"></a>
## B13 — `ShowIndexAsRange` advertises items that do not exist (Medium)

**Where:** [scr/Gobln.Pager.Mvc/PagerFactory.cs:228](../scr/Gobln.Pager.Mvc/PagerFactory.cs#L228)

**What's wrong:** the range upper bound is `(index - 1) * PageSize + PageSize` with no clamp against `TotalItemCount`.

**How it fails (reproduced):** `TotalItemCount = 25, PageSize = 10, TotalPageCount = 3` with `ShowIndexAsRange = true` renders `"1 - 10"`, `"11 - 20"`, `"21 - 30"`. The last label should read `"21 - 25"`.

**Possible solution:**

```csharp
var text = PagerOptions.ShowIndexAsRange
    ? $"{((index - 1) * InternalPage.PageSize) + 1} - {Math.Min(index * InternalPage.PageSize, InternalPage.TotalItemCount)}"
    : index.ToString();
```

---

<a id="b14"></a>
## B14 — the new Blazor test stole the existing `[Fact]` (Medium)

**Where:** [test/Gobln.Pager.BlazorTest/PagerComponentTest02.cs:17-18](../test/Gobln.Pager.BlazorTest/PagerComponentTest02.cs#L17-L18) and [:82](../test/Gobln.Pager.BlazorTest/PagerComponentTest02.cs#L82)

**What's wrong:** `BasicBunit_Test01` was inserted directly beneath the existing `[Fact]` attribute, so it took it over. `InvokePageChange_UpdatesPageIndexAndInvokesCallback` is now an ordinary public method that xUnit never runs.

**How it fails (verified):** `dotnet test test/Gobln.Pager.BlazorTest/` reports `Total: 3` (two from `PagerComponentTest01`, one from `PagerComponentTest02`) — one `[Fact]` for two `public async Task` methods. The orphaned test also contains an assertion that can never pass ([line 133](../test/Gobln.Pager.BlazorTest/PagerComponentTest02.cs#L133) expects a `<span>` at index 3 of `FindAll("a")`), so it would fail if re-enabled — its silence hides that.

**Possible solution:** decide whether the old test is superseded.

- If it is: delete `InvokePageChange_UpdatesPageIndexAndInvokesCallback` — it is a near-duplicate of `BasicBunit_Test01`.
- If it is not: give it its own `[Fact]` and fix line 133 to look up the active page the way the first test does (`cut.Find("span")`), since the active page renders as a `<span>`, not an `<a>`.

Either way, `BasicBunit_Test01` should actually assert what its name promises — it captures `pageChanged` and never checks it, so **nothing currently verifies the `PageIndexChanged` callback at all**:

```csharp
await links[1].ClickAsync(new MouseEventArgs());   // "Previous" from page 3
Assert.Equal(2, pageChanged);
```

That assertion is also what would have caught [B12](#b12).

---

<a id="b15"></a>
## B15 — `AddMvcPager` hard-casts `IPagerOptions` and registers nothing (Medium)

**Where:** [scr/Gobln.Pager.Mvc/PagerServiceCollectionExcetntions.cs:17-24](../scr/Gobln.Pager.Mvc/PagerServiceCollectionExcetntions.cs#L17-L24)

**What's wrong:** the parameter type is the `IPagerOptions` **interface**, but the body does `(PagerOptions)pagerOptions`. The signature advertises support the implementation does not provide. The `services` parameter is never used either — the method only assigns a static, so nothing is resolvable from DI and container lifetimes do not apply. And the instance stored in the static is the caller's own object, which [B06](#b06) then mutates.

**How it fails:** `services.AddMvcPager(myOptions)` where `myOptions` implements `IPagerOptions` without deriving from `PagerOptions` throws `InvalidCastException` at application startup.

**Possible solution:** make the signature honest and stop sharing the caller's instance.

```csharp
public IServiceCollection AddMvcPager(PagerOptions pagerOptions)
{
    ArgumentNullException.ThrowIfNull(pagerOptions);

    PagerSettings.DefaultPagerOptions = pagerOptions.Clone();   // never store the caller's instance

    return services;
}
```

If arbitrary `IPagerOptions` implementations really should be accepted, copy the values across instead of casting (and lift `Clone()` onto the interface as suggested in [B06](#b06)).

Separately: since the method already takes an `IServiceCollection`, consider actually registering the options so consumers can resolve and override them per scope, e.g. `services.TryAddSingleton<IPagerOptions>(_ => PagerSettings.DefaultPagerOptions)`. As written, `AddMvcPager` is a static setter wearing a DI extension's clothes — either register something or document that it is deliberately a global.

---

<a id="minor-cleanups"></a>
## Minor cleanups

| ID | Location | Item |
|----|----------|------|
| M01 | [HtmlExtension.cs:62](../scr/Gobln.Pager.Mvc/HtmlExtension.cs#L62) | `pagerOptions is not null` guards nothing — the value is passed unconditionally to `SetOptions`, which throws `ArgumentNullException`. The compiler already flags it: `warning CS8604` at `HtmlExtension.cs(78,37)`. Either `ArgumentNullException.ThrowIfNull` up front, or drop the check. |
| M02 | [Page.cs:54](../scr/Gobln.Pager/Page.cs#L54) | `_items is null` is dead — `_items` is initialized to `[]` and never assigned null. Also note the double space in `null ||  !_items.Any()`. |
| M03 | [Page.cs:8-9](../scr/Gobln.Pager/Page.cs#L8-L9) | The two `// todo:` comments are stale; the second one ("bug when TotalItemCount is 0 or new Page<>( null array )") is resolved by the [B02](#b02) fix and should be removed with it. |
| M04 | [PagerFactory.cs:386](../scr/Gobln.Pager.Mvc/PagerFactory.cs#L386) | `Math.Min(BoundaryCount, TotalPageCount) - 1 + 1` — dead arithmetic, and the `firstIndex > 0` guard below it is then redundant with the loop condition. |
| M05 | `PagerServiceCollectionExcetntions.cs` | Filename typo ("Excetntions"); the class inside is already named `PagerMvcServiceCollectionExtensions`. Rename the file to match. |
| M06 | [PagerComponentTest02.cs:22](../test/Gobln.Pager.BlazorTest/PagerComponentTest02.cs#L22) | `using var ctx = new BunitContext();` is unused — the class already inherits `BunitContext` and `Render<T>` uses the inherited one. Both test methods are `async Task` with no `await` (`CS1998`). Unused usings: `System.Diagnostics.Metrics`, `System.Reflection`, `Microsoft.AspNetCore.Components`. |
| M07 | [PageDefinition.cs:7](../scr/Gobln.Pager/Infrastructure/PageDefinition.cs#L7) | `PageSize` reports `int.MaxValue` when `pageSize <= 0`, and that value reaches consumers through `Page<T>.PageSize`. It works for `Take()`, but a public `PageSize` of `2147483647` is surprising. Consider reporting `TotalItemCount` instead for the "everything on one page" case. |

---

<a id="suggested-regression-tests"></a>
## Suggested regression tests

None of the 15 findings is covered today. The cheapest tests that would have caught them:

| Test | Catches |
|------|---------|
| `ToPage(pageIndex, pageSize)` for an out-of-range and a last-partial page — assert all four metadata properties | B01, B02 |
| `new PagedList<int>()` → assert `PageCount == 1`, `PageIndex == 1`, `GetPage()` does not throw | B03 |
| `new PagerFactory().Render(page)` and `Render(null)` with each `WhenToShowEnum` value | B04 |
| `HRefUrl` set to `"/products"`, `"products?a=1"`, `"https://x/y"` — assert the generated link | B05 |
| Two successive `html.Pager(page)` calls with different request URLs — assert the second does not reuse the first's URL | B06 |
| Round-trip a `Page<T>` Newtonsoft → Microsoft **and** Microsoft → Newtonsoft | B07 |
| Round-trip with a camel-case contract resolver | B08 |
| Render with each `Show*ButtonLink` set to `Never` **individually** (the current test sets them all at once) | B09 |
| `PageNumbersCreator` for `TotalPageCount` 1–4 × `BoundaryCount` 0–2 — assert no duplicate page numbers | B10 |
| `ToPage` over a counting iterator — assert the source is enumerated at most twice | B11 |
| bUnit: click a page link while the parent keeps `Page.PageIndex` unchanged — assert the pager reverts | B12 |
| `ShowIndexAsRange` on a last partial page | B13 |
| A `[Fact]` count assertion is not practical — but running `dotnet test` and checking the total went from 3 to 4 confirms the fix | B14 |
| `AddMvcPager` with a custom `IPagerOptions` implementation | B15 |

using Gobln.Pager.Json.Newtonsoft;
using Newtonsoft.Json;
using System.Globalization;
using Xunit;

namespace Gobln.Pager.Json.NewtonsoftTest;

/// <summary>
/// The converters read with the caller's serializer, as they write with it: converters and
/// other settings for the items apply in both directions.
/// </summary>
public class SerializerSettingsTest
{
    private sealed class Money
    {
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "";

        public override string ToString() => FormattableString.Invariant($"{Amount} {Currency}");
    }

    /// <summary>
    /// Writes a <see cref="Money"/> as one string, such as "12.5 EUR". Registered in the
    /// settings only, so a serializer without the settings can't read it back.
    /// </summary>
    private sealed class MoneyConverter : JsonConverter<Money>
    {
        public override void WriteJson(JsonWriter writer, Money? value, JsonSerializer serializer)
            => writer.WriteValue(value!.ToString());

        public override Money ReadJson(JsonReader reader, Type objectType, Money? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var parts = ((string)reader.Value!).Split(' ');

            return new Money { Amount = decimal.Parse(parts[0], CultureInfo.InvariantCulture), Currency = parts[1] };
        }
    }

    private static readonly Money[] Prices =
    [
        new() { Amount = 12.5m, Currency = "EUR" },
        new() { Amount = 3m, Currency = "USD" },
    ];

    private static JsonSerializerSettings Settings(JsonConverter pagerConverter) => new()
    {
        Converters = { pagerConverter, new MoneyConverter() },
    };

    [Fact]
    public void Page_UsesTheItemConverterFromTheSettings_WhenReading()
    {
        var settings = Settings(new PageJsonConverter());

        var json = JsonConvert.SerializeObject(Prices.ToPage(1, 10), settings);

        var result = JsonConvert.DeserializeObject<Page<Money>>(json, settings)!;

        Assert.Equal(["12.5 EUR", "3 USD"], result.Select(m => m.ToString()));
    }

    [Fact]
    public void PagedList_UsesTheItemConverterFromTheSettings_WhenReading()
    {
        var settings = Settings(new PagedListJsonConverter());

        var json = JsonConvert.SerializeObject(Prices.ToPagedList(), settings);

        var result = JsonConvert.DeserializeObject<PagedList<Money>>(json, settings)!;

        Assert.Equal(["12.5 EUR", "3 USD"], result.Select(m => m.ToString()));
    }
}

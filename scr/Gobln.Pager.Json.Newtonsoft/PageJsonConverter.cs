using Gobln.Pager;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Gobln.Pager.Json.Newtonsoft;

/// <summary>
/// Converts an <see cref="Page"/> to or from JSON
/// </summary>
public class PageJsonConverter : JsonConverter
{
    // One converter per item type, made by reflection once instead of on every read and write.
    // They hold no state, so all PageJsonConverter instances share them.
    private static readonly ConcurrentDictionary<Type, JsonConverter> Converters = new();

    public override bool CanConvert(Type objectType) => objectType.IsGenericType
                && (objectType.GetGenericTypeDefinition() == typeof(Page<>)
                    || objectType.GetGenericTypeDefinition() == typeof(IPage<>));

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return ConverterFor(objectType.GetGenericArguments()[0]).ReadJson(reader, objectType, existingValue, serializer);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not null)
        {
            ConverterFor(value.GetType().GetGenericArguments()[0]).WriteJson(writer, value, serializer);
        }
    }

    private static JsonConverter ConverterFor(Type elementType) =>
        Converters.GetOrAdd(elementType, static type => (JsonConverter)Activator.CreateInstance(typeof(IPageJsonConverter<>).MakeGenericType(type))!);
}

/// <summary>
/// Converts an <see cref="IPage{T}"/> to or from JSON
/// </summary>
/// <typeparam name="T"></typeparam>
internal class IPageJsonConverter<T> : JsonConverter<IPage<T>>
{
    public override IPage<T>? ReadJson(JsonReader reader, Type objectType, IPage<T>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        // Read with the caller's serializer, as WriteJson writes with it: its settings and
        // converters apply to the items too
        var tempPage = serializer.Deserialize<TempPage<T>>(reader);

        // An empty page without totals is what new Page<T>() writes, with a PageSize of 0 that the
        // constructor below rejects. An empty page with totals goes through it, and keeps them.
        if (tempPage is null || (tempPage.TotalItemCount == 0 && (tempPage.PageItems is null || tempPage.PageItems.Length == 0)))
        {
            return new Page<T>();
        }

        try
        {
            return new Page<T>(tempPage.PageItems ?? [], tempPage.PageIndex, tempPage.PageSize, tempPage.TotalItemCount);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            // The JSON is input: values out of range make it invalid JSON, not an argument error
            throw new JsonSerializationException(ex.Message, ex);
        }
    }

    public override void WriteJson(JsonWriter writer, IPage<T>? value, JsonSerializer serializer)
    {
        TempPage<T> tempPage;

        if (value == null)
        {
            tempPage = new TempPage<T>
            {
                PageIndex = 1,
                PageSize = 0,
                TotalItemCount = 0,
                PageItems = null
            };
        }
        else
        {
            tempPage = new TempPage<T>
            {
                PageIndex = value.PageIndex,
                PageSize = value.PageSize,
                TotalItemCount = value.TotalItemCount,
                PageItems = [.. value]
            };
        }

        serializer.Serialize(writer, tempPage);
    }
}
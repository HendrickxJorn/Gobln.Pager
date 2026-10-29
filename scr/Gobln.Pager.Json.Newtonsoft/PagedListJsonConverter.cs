using Gobln.Pager;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Gobln.Pager.Json.Newtonsoft;

/// <summary>
/// Converts an <see cref="PagedList"/> to or from JSON
/// </summary>
public class PagedListJsonConverter : JsonConverter
{
    // One converter per item type, made by reflection once instead of on every read and write.
    // They hold no state, so all PagedListJsonConverter instances share them.
    private static readonly ConcurrentDictionary<Type, JsonConverter> Converters = new();

    public override bool CanConvert(Type objectType) => objectType.IsGenericType
                && (objectType.GetGenericTypeDefinition() == typeof(PagedList<>)
                    || objectType.GetGenericTypeDefinition() == typeof(IPagedList<>));

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
        Converters.GetOrAdd(elementType, static type => (JsonConverter)Activator.CreateInstance(typeof(IPagedListJsonConverter<>).MakeGenericType(type))!);
}

/// <summary>
/// Converts an <see cref="IPagedList{T}"/> to or from JSON
/// </summary>
/// <typeparam name="T"></typeparam>
internal class IPagedListJsonConverter<T> : JsonConverter<IPagedList<T>>
{
    public override IPagedList<T>? ReadJson(JsonReader reader, Type objectType, IPagedList<T>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        // Read with the caller's serializer, as WriteJson writes with it: its settings and
        // converters apply to the items too
        var items = serializer.Deserialize<IEnumerable<T>>(reader);

        // Only the items are in the JSON, so the list gets the default page size and is on page 1
        return new PagedList<T>(items ?? []);
    }

    public override void WriteJson(JsonWriter writer, IPagedList<T>? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value?.ToArray());
    }
}
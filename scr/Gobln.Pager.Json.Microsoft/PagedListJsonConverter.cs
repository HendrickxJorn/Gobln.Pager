using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gobln.Pager.Json.Microsoft;

/// <summary>
/// Converts an <see cref="PagedList"/> to or from JSON
/// </summary>
public class PagedListJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsGenericType
            && (typeToConvert.GetGenericTypeDefinition() == typeof(PagedList<>)
                || typeToConvert.GetGenericTypeDefinition() == typeof(IPagedList<>));

    public override JsonConverter CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        Type elementType = typeToConvert.GetGenericArguments()[0];

        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(IPagedListJsonConverter<>)
                .MakeGenericType(elementType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null)!;

        return converter;
    }
}

/// <summary>
/// Converts an <see cref="IPagedList{T}"/> to or from JSON
/// </summary>
/// <typeparam name="T"></typeparam>
internal class IPagedListJsonConverter<T> : JsonConverter<IPagedList<T>>
{
    public override IPagedList<T> Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        var items = JsonSerializer.Deserialize<IEnumerable<T>>(ref reader, options);

        // Only the items are in the JSON, so the list gets the default page size and is on page 1
        return new PagedList<T>(items ?? []);
    }

    public override void Write(
        Utf8JsonWriter writer, IPagedList<T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.ToArray(), options);
    }
}
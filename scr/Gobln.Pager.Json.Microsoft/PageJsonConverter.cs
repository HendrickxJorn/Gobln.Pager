using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gobln.Pager.Json.Microsoft;

/// <summary>
/// Converts <see cref="Page"/> to or from JSON
/// </summary>
public class PageJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsGenericType
                    && (typeToConvert.GetGenericTypeDefinition() == typeof(Page<>)
                        || typeToConvert.GetGenericTypeDefinition() == typeof(IPage<>));

    public override JsonConverter CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        Type elementType = typeToConvert.GetGenericArguments()[0];

        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(IPageJsonConverter<>)
                .MakeGenericType(elementType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null)!;

        return converter;
    }
}

/// <summary>
/// Converts <see cref="IPage{T}"/> to or from JSON
/// </summary>
/// <typeparam name="T"></typeparam>
internal class IPageJsonConverter<T> : JsonConverter<IPage<T>>
{
    public override IPage<T> Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        // Deserialize consumes the whole object and leaves the reader on its EndObject,
        // which is the position a converter is required to return on. Do not Read() past it.
        var temp = JsonSerializer.Deserialize<TempPage<T>>(ref reader, options);

        return temp?.PageItems is null || temp.PageItems.Length == 0
            ? new Page<T>()
            : new Page<T>(temp.PageItems, temp.PageIndex, temp.PageSize, temp.TotalItemCount);
    }

    public override void Write(
        Utf8JsonWriter writer, IPage<T> value, JsonSerializerOptions options)
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

        JsonSerializer.Serialize(writer, tempPage, options);
    }
}
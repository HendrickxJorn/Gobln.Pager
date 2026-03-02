using Gobln.Pager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Gobln.Pager.Json.Newtonsoft;

/// <summary>
/// Converts an <see cref="PagedList"/> to or from JSON
/// </summary>
public class PagedListJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType.IsGenericType
                && (objectType.GetGenericTypeDefinition() == typeof(PagedList<>)
                    || objectType.GetGenericTypeDefinition() == typeof(IPagedList<>));

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        Type elementType = objectType.GetGenericArguments()[0];

        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(IPagedListJsonConverter<>)
                .MakeGenericType([elementType]),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null)!;

        return converter.ReadJson(reader, objectType, existingValue, serializer);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not null)
        {
            Type elementType = value.GetType().GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(IPagedListJsonConverter<>)
                    .MakeGenericType([elementType]),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null)!;

            converter.WriteJson(writer, value, serializer);
        }
    }
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

        var jsonObj = JToken.ReadFrom(reader);

        var tempPage = jsonObj.ToObject<IEnumerable<T>>();

        return tempPage is null || tempPage.Count() == 0
           ? new PagedList<T>()
           : new PagedList<T>(tempPage);
    }

    public override void WriteJson(JsonWriter writer, IPagedList<T>? value, JsonSerializer serializer)
    {
        writer.WriteRawValue(JsonConvert.SerializeObject(value));
    }
}
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gobln.Pager.Json.Microsoft
{

    // for interface
    // https://github.com/dotnet/runtime/issues/46920
    // https://stackoverflow.com/questions/24644464/type-is-an-interface-or-abstract-class-and-cannot-be-instantiated


    // https://stackoverflow.com/questions/77137648/unable-to-get-jsonconverterfactory-working-in-system-text-json

    // check code massetransit for maybe more info

    // https://gist.github.com/tonysneed/5e7988516b081d454cde95b5d729e1af

    /// <summary>
    /// Converts an <see cref="Page"/> to or from JSON
    /// </summary>
    public class PageJsonConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)  => typeToConvert.IsGenericType
                        && (typeToConvert.GetGenericTypeDefinition() == typeof(Page<>)
                            || typeToConvert.GetGenericTypeDefinition() == typeof(IPage<>));

        public override JsonConverter CreateConverter(
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            Type elementType = typeToConvert.GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(PageJsonConverter<>)
                    .MakeGenericType(elementType),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null)!;

            return converter;
        }
    }

    /// <summary>
    /// Converts an <see cref="Page{T}"/> to or from JSON
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageJsonConverter<T> : JsonConverter<Page<T>>
    {
        public override Page<T> Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            reader.Read();

            var temp = JsonSerializer.Deserialize<TempPage<T>>(ref reader, options)!;

            var elements = temp.PageItems?.Length == 0
                ? new Page<T>()
                : new Page<T>(temp.PageItems, temp.PageIndex, temp.PageSize, temp.TotalItemCount);

            reader.Read();

            return elements;
        }

        public override void Write(
            Utf8JsonWriter writer, Page<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            TempPage<T> tempPage;

            if (value == null)
            {
                tempPage = new TempPage<T>
                {
                    PageIndex = 1,
                    PageSize = int.MaxValue,
                    TotalItemCount = 1,
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

            writer.WriteEndArray();
        }
    }

    /// <summary>
    /// Converts an <see cref="IPage{T}"/> to or from JSON
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IPageJsonConverter<T> : JsonConverter<IPage<T>>
    {
        public override IPage<T> Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            reader.Read();

            var temp = JsonSerializer.Deserialize<TempPage<T>>(ref reader, options)!;

            var elements = temp.PageItems?.Length == 0 
                ? new Page<T>()
                : new Page<T>(temp.PageItems, temp.PageIndex, temp.PageSize, temp.TotalItemCount);

            reader.Read();

            return elements;
        }

        public override void Write(
            Utf8JsonWriter writer, IPage<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

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

            writer.WriteEndArray();
        }
    }
}
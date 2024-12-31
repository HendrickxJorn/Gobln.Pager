using Gobln.Pager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Gobln.Pager.Json.Newtonsoft
{
    /// <summary>
    /// Converts an <see cref="Page"/> to or from JSON
    /// </summary>
    public class PageJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsGenericType
                    && (objectType.GetGenericTypeDefinition() == typeof(Page<>)
                        || objectType.GetGenericTypeDefinition() == typeof(IPage<>));

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            Type elementType = objectType.GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(IPageJsonConverter<>)
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
                    typeof(IPageJsonConverter<>)
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

            var jsonObj = JToken.ReadFrom(reader);

            var tempPage = jsonObj.ToObject<TempPage<T>>();

            return tempPage is null || tempPage.PageItems?.Length == 0
               ? new Page<T>()
               : new Page<T>(tempPage.PageItems, tempPage.PageIndex, tempPage.PageSize, tempPage.TotalItemCount);
        }

        public override void WriteJson(JsonWriter writer, IPage<T>? value, JsonSerializer serializer)
        {
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

            writer.WriteRawValue(JsonConvert.SerializeObject(tempPage));
        }
    }
}
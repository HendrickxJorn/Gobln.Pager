using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gobln.Pager;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Golbn.Pager.Json.Newtonsoft
{
    //https://www.newtonsoft.com/json/help/html/CustomJsonConverter.htm

    //https://www.newtonsoft.com/json/help/html/CustomJsonConverterGeneric.htm

    /// <summary>
    /// Converts an <see cref="Page"/> to or from JSON
    /// </summary>
    public class PageJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsGenericType
                && objectType.GetGenericTypeDefinition() == typeof(Page<>);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            Type elementType = objectType.GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(PageJsonConverter<>)
                    .MakeGenericType([elementType]),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null)!;

            return converter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            Type elementType = value.GetType().GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(PageJsonConverter<>)
                    .MakeGenericType([elementType]),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null)!;

            converter.WriteJson(writer, value, serializer);
        }
    }

    /// <summary>
    /// Converts an <see cref="Page{T}"/> to or from JSON
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageJsonConverter<T> : JsonConverter<Page<T>>
    {
        public override Page<T>? ReadJson(JsonReader reader, Type objectType, Page<T>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObj = JToken.ReadFrom(reader);

            var tempPage = jsonObj.ToObject<TempPage<T>>();

            return new Page<T>(tempPage.PageItems, tempPage.PageIndex, tempPage.PageSize, tempPage.TotalItemCount);
        }

        public override void WriteJson(JsonWriter writer, Page<T>? value, JsonSerializer serializer)
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

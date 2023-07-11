using System.Text.Json;

namespace ResponseProcessor
{
    public static class JsonExtensions
    {
        public static JsonElement? SelectToken(this JsonElement element, string path)
        {
            string[] parts = path.Split('.');
            JsonElement? currentElement = element;

            foreach (string part in parts)
            {
                if (currentElement == null || !currentElement.HasValue || currentElement.Value.ValueKind != JsonValueKind.Object)
                {
                    return null;
                }

                currentElement = currentElement.Value.TryGetProperty(part, out JsonElement propertyValue)
                    ? propertyValue
                    : null;
            }

            return currentElement;
        }
    }

    public static class JsonConverter
    {
        public static IEnumerable<KeyValuePair<string, string>> ConvertJsonToKeyValuePairs(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            using var jsonDocument = JsonDocument.Parse(jsonString);

            return ConvertJsonToKeyValuePairs(jsonDocument.RootElement);
        }

        public static IEnumerable<KeyValuePair<string, string>> ConvertJsonToKeyValuePairs(JsonElement jsonElement)
        {
            if (jsonElement.ValueKind != JsonValueKind.Object)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            var dictionary = new Dictionary<string, string>();

            foreach (var property in jsonElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    // 如果属性值是一个嵌套的对象，则递归转换并添加前缀
                    var nestedPairs = ConvertJsonToKeyValuePairs(property.Value);
                    foreach (var nestedPair in nestedPairs)
                    {
                        var key = $"{property.Name}.{nestedPair.Key}";
                        dictionary[key] = nestedPair.Value;
                    }
                }
                else
                {
                    // 如果属性值不是对象，则直接添加键值对
                    dictionary[property.Name] = property.Value.ToString();
                }
            }

            return dictionary;
        }
    }
}

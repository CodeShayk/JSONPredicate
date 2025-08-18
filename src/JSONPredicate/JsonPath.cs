using System.Linq;
using System.Text.Json;

namespace JsonPathPredicate
{
    internal static class JsonPath
    {
        public static object Evaluate(object obj, string path)
        {
            var json = JsonSerializer.Serialize(obj);
            using (var document = JsonDocument.Parse(json))
            {
                var element = document.RootElement;

                var parts = path.Split('.').Where(p => !string.IsNullOrEmpty(p)).ToArray();
                foreach (var part in parts)
                {
                    if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(part, out element))
                        return null;
                }

                return DeserializeElement(element);
            }
        }

        private static object DeserializeElement(JsonElement element)
        {
            object result = null;
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    result = DataTypes.TryParseDateTime(element.GetString()) ?? element.GetString();
                    break;

                case JsonValueKind.Number:
                    result = element.TryGetInt32(out var i) ? i : element.GetDouble();
                    break;

                case JsonValueKind.True:
                    result = true;
                    break;

                case JsonValueKind.False:
                    result = false;
                    break;

                case JsonValueKind.Array:
                    result = element.EnumerateArray().Select(DeserializeElement).ToArray();
                    break;

                default:
                    result = null;
                    break;
            }
            ;

            return result;
        }
    }
}
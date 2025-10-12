using System.Linq;
using System.Text.Json;
using System.Reflection;

namespace JSONPredicate
{
    internal static class JsonPath
    {
        public static object Evaluate(object obj, string path)
        {
            if (obj == null) return null;
            
            var properties = path.Split('.');
            object current = obj;
            
            foreach (var property in properties)
            {
                if (current == null) return null;
                
                // Check if it's an indexed access: array[0] or list[1]
                if (property.Contains("[") && property.EndsWith("]"))
                {
                    var parts = property.Split('[');
                    var propName = parts[0];
                    var indexStr = parts[1].TrimEnd(']');
                    
                    // First get the property
                    var propInfo = current.GetType().GetProperty(propName, 
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null) return null;
                    
                    var propValue = propInfo.GetValue(current);
                    
                    // Then access array/list element if applicable
                    if (propValue is System.Collections.IList list)
                    {
                        if (int.TryParse(indexStr, out int index) && index >= 0 && index < list.Count)
                        {
                            current = list[index];
                        }
                        else
                        {
                            return null; // Invalid index
                        }
                    }
                    else
                    {
                        return null; // Not an indexable type
                    }
                }
                else
                {
                    var propInfo = current.GetType().GetProperty(property, 
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null) return null;
                    
                    current = propInfo.GetValue(current);
                }
            }
            
            return current;
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
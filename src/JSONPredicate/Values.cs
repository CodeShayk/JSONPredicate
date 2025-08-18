using System;
using System.Collections.Generic;

namespace JsonPathPredicate
{
    internal static class Values
    {
        public static object Parse(string value, Type targetType)
        {
            value = value.Trim();

            // Handle IN operator values (parentheses format)
            if (value.StartsWith("(") && value.EndsWith(")"))
            {
                var items = ParseInOperatorValues(value.Substring(1, value.Length - 2));
                return items;
            }

            return ParseSingleValue(value, targetType);
        }

        private static IEnumerable<object> ParseInOperatorValues(string value)
        {
            var items = new List<string>();
            var currentItem = "";
            var inQuotes = false;
            var quoteChar = '\0';

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];

                if (!inQuotes && (c == '\'' || c == '"' || c == '`'))
                {
                    inQuotes = true;
                    quoteChar = c;
                    currentItem += c;
                    continue;
                }

                if (inQuotes && c == quoteChar)
                {
                    inQuotes = false;
                    quoteChar = '\0';
                    currentItem += c;
                    continue;
                }

                if (!inQuotes && c == ',')
                {
                    items.Add(currentItem.Trim());
                    currentItem = "";
                    continue;
                }

                currentItem += c;
            }

            if (!string.IsNullOrWhiteSpace(currentItem))
            {
                items.Add(currentItem.Trim());
            }

            // Parse each item based on its content
            var parsedItems = new List<object>();
            foreach (var item in items)
            {
                var trimmedItem = item.Trim();
                // Remove quotes if present
                if ((trimmedItem.StartsWith("'") && trimmedItem.EndsWith("'")) ||
                    (trimmedItem.StartsWith("`") && trimmedItem.EndsWith("`")) ||
                    (trimmedItem.StartsWith("\"") && trimmedItem.EndsWith("\"")))
                {
                    trimmedItem = trimmedItem.Substring(1, trimmedItem.Length - 2);
                }
                parsedItems.Add(trimmedItem);
            }

            return parsedItems;
        }

        private static object ParseSingleValue(string value, Type targetType)
        {
            // Remove quotes if present
            if ((value.StartsWith("'") && value.EndsWith("'")) ||
                (value.StartsWith("`") && value.EndsWith("`")) ||
                (value.StartsWith("\"") && value.EndsWith("\"")))
            {
                value = value.Substring(1, value.Length - 2);
            }

            // Handle DateTime
            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?) ||
                (targetType == null && DataTypes.IsLikelyDateTime(value)))
            {
                return DataTypes.ParseDateTime(value);
            }

            if (targetType == typeof(string) || targetType == null)
                return value;

            if (targetType == typeof(int) || targetType == typeof(int?))
                return int.Parse(value);

            if (targetType == typeof(bool) || targetType == typeof(bool?))
                return bool.Parse(value);

            if (targetType == typeof(double) || targetType == typeof(double?))
                return double.Parse(value);

            return value;
        }
    }
}
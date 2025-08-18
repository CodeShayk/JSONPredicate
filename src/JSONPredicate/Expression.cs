using System;
using System.Text.RegularExpressions;

namespace JSONPredicate
{
    internal static class Expression
    {
        public static class Logical
        {
            public const string AndOperator = "and";
            public const string OrOperator = "or";
        }

        public static class Comparison
        {
            public const string NotOperator = "not";
            public const string InOperator = "in";
            public const string EqOperator = "eq";
            public const string GtOperator = "gt";
            public const string GteOperator = "gte";
            public const string LtOperator = "lt";
            public const string LteOperator = "lte";
        }

        public static (string Path, string Operator, string Value) Parse(string expression)
        {
            var pattern = @"^(.+?)\s+(eq|in|not|gt|gte|lt|lte)\s+(.+)$";
            var match = Regex.Match(expression.Trim(), pattern);

            if (!match.Success)
                throw new ArgumentException($"Invalid expression format: {expression}");

            return (match.Groups[1].Value.Trim(), match.Groups[2].Value, match.Groups[3].Value.Trim());
        }
    }
}
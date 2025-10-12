using System;

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
            public const string StartsWithOperator = "starts_with";
            public const string EndsWithOperator = "ends_with";
            public const string ContainsOperator = "contains";
        }

        public static (string Path, string Operator, string Value) Parse(string expression)
        {
            var expr = expression.Trim();
            if (string.IsNullOrEmpty(expr))
                throw new ArgumentException($"Invalid expression format: {expression}");

            // Define operators in order of length (longer first) to avoid partial matches
            var operators = new[] { "gte", "lte", "not", "eq", "gt", "lt", "in", "starts_with", "ends_with", "contains" };

            for (int i = 0; i < expr.Length; i++)
            {
                // Skip if inside quotes
                if (expr[i] == '\'' || expr[i] == '"' || expr[i] == '`')
                {
                    var quoteChar = expr[i];
                    i++;
                    while (i < expr.Length && expr[i] != quoteChar)
                    {
                        if (i + 1 < expr.Length && expr[i] == '\\') // Handle escaped quotes
                        {
                            i += 2;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    continue;
                }

                // Check for operator at this position (not in quotes)
                foreach (var op in operators)
                {
                    if (i + op.Length <= expr.Length &&
                        expr.Substring(i, op.Length).Equals(op, StringComparison.OrdinalIgnoreCase))
                    {
                        // Verify it's surrounded by whitespace or string boundaries
                        bool beforeOk = i == 0 || char.IsWhiteSpace(expr[i - 1]);
                        bool afterOk = i + op.Length == expr.Length || char.IsWhiteSpace(expr[i + op.Length]);

                        if (beforeOk && afterOk)
                        {
                            var path = expr.Substring(0, i).Trim();
                            var value = expr.Substring(i + op.Length).Trim();

                            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(value))
                                throw new ArgumentException($"Invalid expression format: {expression}");

                            return (path, op, value);
                        }
                    }
                }
            }

            throw new ArgumentException($"Invalid expression format: {expression}");
        }
    }
}
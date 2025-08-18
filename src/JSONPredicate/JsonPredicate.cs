using System;
using System.Collections.Generic;
using System.Linq;
using JsonPathPredicate;
using JsonPathPredicate.Operators;

namespace JSONPredicate
{
    public static class JSONPredicate
    {
        private static readonly Dictionary<string, Func<object, object, bool>> ComparisonOperators = new Dictionary<string, Func<object, object, bool>>()
        {
            { Expression.Comparison.EqOperator, (left, right) => EqOperator.Evaluate(left, right)},
            { Expression.Comparison.InOperator, (left, right) => InOperator.Evaluate(left, right) },
            { Expression.Comparison.NotOperator, (left, right) => NotOperator.Evaluate(left, right) },
            { Expression.Comparison.GtOperator, (left, right) => GtOperator.Evaluate(left, right) },
            { Expression.Comparison.GteOperator, (left, right) => GteOperator.Evaluate(left, right) },
            { Expression.Comparison.LtOperator, (left, right) => LtOperator.Evaluate(left, right) },
            { Expression.Comparison.LteOperator, (left, right) => LteOperator.Evaluate(left, right)}
        };

        public static bool Evaluate(string expression, object obj)
        {
            // Handle OR operations first (lower precedence)

            var orParts = SplitByOperator(expression, Expression.Logical.OrOperator);
            if (orParts.Length > 1)
            {
                return orParts.Any(part => Evaluate(part.Trim(), obj));
            }

            // Handle AND operations
            var andParts = SplitByOperator(expression, Expression.Logical.AndOperator);
            if (andParts.Length > 1)
            {
                return andParts.All(part => Evaluate(part.Trim(), obj));
            }

            // Handle simple expressions (with potential grouping)
            expression = expression.Trim();
            if (expression.StartsWith("(") && expression.EndsWith(")"))
            {
                return Evaluate(expression.Substring(1, expression.Length - 2), obj);
            }

            // Handle atomic expressions
            return EvaluateAtomicExpression(expression, obj);
        }

        private static string[] SplitByOperator(string expression, string op)
        {
            var parts = new List<string>();
            int parenthesesCount = 0;
            int lastSplit = 0;

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];
                if (c == '(')
                    parenthesesCount++;
                else if (c == ')')
                    parenthesesCount--;
                else if (parenthesesCount == 0 &&
                         i + op.Length <= expression.Length &&
                         expression.Substring(i, op.Length).Equals(op, StringComparison.OrdinalIgnoreCase) &&
                         (i == 0 || char.IsWhiteSpace(expression[i - 1])) &&
                         (i + op.Length >= expression.Length || char.IsWhiteSpace(expression[i + op.Length])))
                {
                    parts.Add(expression.Substring(lastSplit, i - lastSplit));
                    i += op.Length - 1; // Skip the operator
                    lastSplit = i + 1;
                }
            }

            parts.Add(expression.Substring(lastSplit));
            return parts.ToArray();
        }

        private static bool EvaluateAtomicExpression(string expression, object obj)
        {
            var (Path, Operator, Value) = Expression.Parse(expression);
            var actual = JsonPath.Evaluate(obj, Path);
            var expected = Values.Parse(Value, actual?.GetType() ?? typeof(string));

            return ComparisonOperators[Operator](actual, expected);
        }
    }
}
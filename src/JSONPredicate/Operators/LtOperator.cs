namespace JsonPathPredicate.Operators
{
    internal static class LtOperator
    {
        public static bool Evaluate(object left, object right) => DataTypes.CompareValues(left, right) < 0;
    }
}
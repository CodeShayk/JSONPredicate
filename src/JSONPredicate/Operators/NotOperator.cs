namespace JsonPathPredicate.Operators
{
    internal static class NotOperator
    {
        public static bool Evaluate(object left, object right) => !DataTypes.AreEqual(left, right);
    }
}
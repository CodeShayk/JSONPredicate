namespace JsonPathPredicate.Operators
{
    internal static class EqOperator
    {
        public static bool Evaluate(object left, object right) => DataTypes.AreEqual(left, right);
    }
}
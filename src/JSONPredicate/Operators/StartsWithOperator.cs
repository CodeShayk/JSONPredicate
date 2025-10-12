namespace JSONPredicate.Operators
{
    internal static class StartsWithOperator
    {
        public static bool Evaluate(object left, object right)
        {
            if (left == null || right == null) return false;
            var leftStr = left.ToString();
            var rightStr = right.ToString();
            return leftStr.StartsWith(rightStr, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
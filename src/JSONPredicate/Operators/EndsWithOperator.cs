namespace JSONPredicate.Operators
{
    internal static class EndsWithOperator
    {
        public static bool Evaluate(object left, object right)
        {
            if (left == null || right == null)
                return false;
            var leftStr = left.ToString();
            var rightStr = right.ToString();
            return leftStr.EndsWith(rightStr, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
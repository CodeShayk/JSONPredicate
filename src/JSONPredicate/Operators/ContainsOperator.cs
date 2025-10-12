namespace JSONPredicate.Operators
{
    internal static class ContainsOperator
    {
        public static bool Evaluate(object left, object right)
        {
            if (left == null || right == null)
                return false;
            var leftStr = left.ToString();
            var rightStr = right.ToString();
            return leftStr.IndexOf(rightStr, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
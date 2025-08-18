using System.Collections;

namespace JsonPathPredicate.Operators
{
    internal static class InOperator
    {
        public static bool Evaluate(object left, object right)
        {
            // Handle null cases
            if (left == null && right == null)
                return true;
            if (left == null || right == null)
                return false;

            // Direct equality check first
            if (DataTypes.AreEqual(left, right))
                return true;

            // Check if left is single value, right is collection
            if (!IsEnumerableExceptString(left) && IsEnumerableExceptString(right))
                return ContainsValue((IEnumerable)right, left);

            // Check if right is single value, left is collection
            if (!IsEnumerableExceptString(right) && IsEnumerableExceptString(left))
                return ContainsValue((IEnumerable)left, right);

            // Both are collections - check for any intersection
            if (IsEnumerableExceptString(left) && IsEnumerableExceptString(right))
                return HasIntersection((IEnumerable)left, (IEnumerable)right);

            return false;
        }

        private static bool IsEnumerableExceptString(object obj)
        {
            return obj is IEnumerable && !(obj is string);
        }

        private static bool ContainsValue(IEnumerable collection, object value)
        {
            foreach (var item in collection)
                if (DataTypes.AreEqual(item, value))
                    return true;
            return false;
        }

        private static bool HasIntersection(IEnumerable left, IEnumerable right)
        {
            foreach (var leftItem in left)
                foreach (var rightItem in right)
                    if (DataTypes.AreEqual(leftItem, rightItem))
                        return true;
            return false;
        }
    }
}
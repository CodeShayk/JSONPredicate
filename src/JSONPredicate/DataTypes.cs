using System;
using System.Globalization;

namespace JsonPathPredicate
{
    internal static class DataTypes
    {
        public static int CompareValues(object left, object right)
        {
            if (left == null && right == null)
                return 0;
            if (left == null)
                return -1;
            if (right == null)
                return 1;

            // Handle DateTime comparisons
            if (left is DateTime leftDateTime && right is DateTime rightDateTime)
            {
                return leftDateTime.CompareTo(rightDateTime);
            }

            // Handle mixed DateTime comparisons
            if (left is DateTime || right is DateTime)
            {
                try
                {
                    var leftDt = left is DateTime ? (DateTime)left : ParseDateTime(left.ToString());
                    var rightDt = right is DateTime ? (DateTime)right : ParseDateTime(right.ToString());
                    return leftDt.CompareTo(rightDt);
                }
                catch
                {
                    // Fall through to string comparison if DateTime parsing fails
                }
            }

            // Handle numeric comparisons
            if (left is IComparable leftComparable && right is IComparable rightComparable)
            {
                try
                {
                    return leftComparable.CompareTo(rightComparable);
                }
                catch
                {
                    // If direct comparison fails, convert to double for comparison
                    if (double.TryParse(left.ToString(), out var leftDouble) &&
                        double.TryParse(right.ToString(), out var rightDouble))
                    {
                        return leftDouble.CompareTo(rightDouble);
                    }
                }
            }

            // String comparison as fallback
            return string.Compare(left.ToString(), right.ToString(), StringComparison.Ordinal);
        }

        public static bool AreEqual(object left, object right)
        {
            if (left == null && right == null)
                return true;

            if (left == null || right == null)
                return false;

            // Handle string comparison with case insensitivity
            if (left is string leftStr && right is string rightStr)
            {
                return leftStr.Equals(rightStr, StringComparison.OrdinalIgnoreCase);
            }

            // Handle char comparison (case insensitive)
            if (left is char leftChar && right is char rightChar)
            {
                return char.ToUpperInvariant(leftChar) == char.ToUpperInvariant(rightChar);
            }

            // Handle cross-type string/char comparison
            if (left is string str && right is char ch)
            {
                return str.Length == 1 && char.ToUpperInvariant(str[0]) == char.ToUpperInvariant(ch);
            }
            if (left is char ch2 && right is string str2)
            {
                return str2.Length == 1 && char.ToUpperInvariant(ch2) == char.ToUpperInvariant(str2[0]);
            }

            // Handle numeric types with conversion
            if (IsNumericType(left) && IsNumericType(right))
            {
                return CompareNumericValues(left, right);
            }

            // Handle boolean comparisons
            if (left is bool leftBool && right is bool rightBool)
            {
                return leftBool == rightBool;
            }

            // Handle DateTime comparisons
            if (left is DateTime leftDate && right is DateTime rightDate)
            {
                return leftDate == rightDate;
            }

            // Handle DateTimeOffset comparisons
            if (left is DateTimeOffset leftOffset && right is DateTimeOffset rightOffset)
            {
                return leftOffset == rightOffset;
            }

            // Handle TimeSpan comparisons
            if (left is TimeSpan leftSpan && right is TimeSpan rightSpan)
            {
                return leftSpan == rightSpan;
            }

            // Handle Guid comparisons
            if (left is Guid leftGuid && right is Guid rightGuid)
            {
                return leftGuid == rightGuid;
            }

            // Handle enum comparisons
            if (left.GetType().IsEnum && right.GetType().IsEnum)
            {
                // Convert both to underlying type for comparison
                if (left.GetType() == right.GetType())
                {
                    return left.Equals(right);
                }
                // Different enum types - compare underlying values
                return Convert.ToInt64(left) == Convert.ToInt64(right);
            }

            // Handle string representation fallback for primitives
            if (IsPrimitiveType(left) && IsPrimitiveType(right))
            {
                return left.ToString().Equals(right.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            // Default equality check
            return Equals(left, right);
        }

        private static bool IsNumericType(object obj)
        {
            return obj is sbyte || obj is byte || obj is short || obj is ushort ||
                   obj is int || obj is uint || obj is long || obj is ulong ||
                   obj is float || obj is double || obj is decimal;
        }

        private static bool IsPrimitiveType(object obj)
        {
            if (obj == null)
                return false;
            var type = obj.GetType();
            return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) ||
                   type == typeof(DateTime) || type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) || type == typeof(Guid) || type.IsEnum;
        }

        private static bool CompareNumericValues(object left, object right)
        {
            try
            {
                // Convert both to decimal for precise comparison
                // Handle special floating point cases first
                if (left is float leftFloat && (float.IsNaN(leftFloat) || float.IsInfinity(leftFloat)))
                {
                    if (right is float rightFloat)
                        return leftFloat.Equals(rightFloat);
                    if (right is double rightDouble)
                        return ((double)leftFloat).Equals(rightDouble);
                    return false;
                }

                if (left is double leftDouble && (double.IsNaN(leftDouble) || double.IsInfinity(leftDouble)))
                {
                    if (right is double rightDouble)
                        return leftDouble.Equals(rightDouble);
                    if (right is float rightFloat)
                        return leftDouble.Equals((double)rightFloat);
                    return false;
                }

                if (right is float rightF && (float.IsNaN(rightF) || float.IsInfinity(rightF)))
                {
                    if (left is float leftF)
                        return rightF.Equals(leftF);
                    if (left is double leftD)
                        return ((double)rightF).Equals(leftD);
                    return false;
                }

                if (right is double rightD && (double.IsNaN(rightD) || double.IsInfinity(rightD)))
                {
                    if (left is double leftD)
                        return rightD.Equals(leftD);
                    if (left is float leftF)
                        return rightD.Equals((double)leftF);
                    return false;
                }

                // For normal numeric values, use decimal conversion
                decimal leftDecimal = Convert.ToDecimal(left, CultureInfo.InvariantCulture);
                decimal rightDecimal = Convert.ToDecimal(right, CultureInfo.InvariantCulture);
                return leftDecimal == rightDecimal;
            }
            catch (OverflowException)
            {
                // If conversion to decimal fails due to overflow, fall back to double
                try
                {
                    double leftDouble = Convert.ToDouble(left, CultureInfo.InvariantCulture);
                    double rightDouble = Convert.ToDouble(right, CultureInfo.InvariantCulture);
                    return Math.Abs(leftDouble - rightDouble) < double.Epsilon;
                }
                catch
                {
                    // Last resort: string comparison
                    return left.ToString().Equals(right.ToString(), StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                // Fallback to standard equality
                return Equals(left, right);
            }
        }

        public static DateTime ParseDateTime(string value)
        {
            // Try parsing with different formats
            var formats = new[]
            {
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                "yyyy-MM-ddTHH:mm:sszzz",
                "yyyy-MM-ddTHH:mm:ss.fffzzz",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd",
                "MM/dd/yyyy",
                "MM/dd/yyyy HH:mm:ss"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                {
                    return dateTime;
                }
            }

            // Try general parsing as fallback
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var generalDateTime))
            {
                return generalDateTime;
            }

            throw new FormatException($"Unable to parse DateTime value: {value}");
        }

        public static object TryParseDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var formats = new[]
            {
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                "yyyy-MM-ddTHH:mm:sszzz",
                "yyyy-MM-ddTHH:mm:ss.fffzzz"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                {
                    return dateTime;
                }
            }

            return null;
        }

        public static bool IsLikelyDateTime(string value)
        {
            return value.Contains("T") ||
                   (value.Length == 10 && value[4] == '-' && value[7] == '-') ||
                   value.EndsWith("Z") ||
                   value.Contains("+");
        }
    }
}
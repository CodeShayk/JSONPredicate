namespace JSONPredicate.Tests
{
    [TestFixture]
    public class PredicateTests
    {
        private readonly object _testObject = new
        {
            client = new
            {
                address =

                new
                {
                    postcode = "e113et",
                    number = 123,
                    active = true
                },
                tags = new[] { "vip", "premium" },
                name = "John",
                role = "admin"
            },
            score = 95.5,
            status = "active",
            age = 30,
            price = 29.99,
            createdAt = DateTime.Parse("2023-01-15T10:30:00Z"),
            lastLogin = "2023-12-01T14:22:33Z", // String representation
            expiryDate = DateTime.Parse("2024-12-31T23:59:59Z")
        };

        // Basic comparison tests
        [Test]
        public void Evaluate_EqOperatorWithString_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode eq `e113et`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_EqOperatorWithString_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode eq `wrong`", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_EqOperatorWithNumber_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.address.number eq 123", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_EqOperatorWithBoolean_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.address.active eq true", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_NotOperator_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode not `wrong`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InOperatorWithParentheses_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.tags in (`vip`, `premium`)", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InOperatorWithParentheses_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("client.tags in (`basic`, `standard`)", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_InOperatorWithSingleValue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.role in (`admin`)", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InOperatorWithMultipleValuesNoneMatch_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("client.role in (`user`, `guest`, `moderator`)", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_InOperatorWithStringQuotes_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.name in (`John`, `Jane`, `Bob`)", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_NestedPropertyNotFound_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("client.unknown.property eq `value`", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_WithDoubleValue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("score eq 95.5", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InvalidExpressionFormat_ShouldThrowException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                JSONPredicate.Evaluate("invalid expression", _testObject));
            Assert.That(ex.Message, Does.Contain("Invalid expression format"));
        }

        // Logical operator tests
        [Test]
        public void Evaluate_AndOperator_BothTrue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode eq `e113et` and client.address.number eq 123", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_AndOperator_OneFalse_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode eq `e113et` and client.address.number eq 999", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_OrOperator_BothTrue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode eq `e113et` or client.address.number eq 999", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_OrOperator_OneTrue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode eq `wrong` or client.address.number eq 123", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_OrOperator_BothFalse_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode eq `wrong` or client.address.number eq 999", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_ComplexAndOrExpression_TrueAndTrueOrFalse_ShouldReturnTrue()
        {
            // (postcode eq 'e113et' and number eq 123) or (name eq 'Jane')
            // (true and true) or false = true
            var result = JSONPredicate.Evaluate("client.address.postcode eq `e113et` and client.address.number eq 123 or client.name eq `Jane`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_ComplexAndOrExpression_FalseAndTrueOrFalse_ShouldReturnFalse()
        {
            // (postcode eq 'wrong' and number eq 123) or (name eq 'Jane')
            // (false and true) or false = false
            var result = JSONPredicate.Evaluate("client.address.postcode eq `wrong` and client.address.number eq 123 or client.name eq `Jane`", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_ParenthesesOverridePrecedence_ShouldReturnFalse()
        {
            // postcode eq 'wrong' and (number eq 123 or name eq 'Jane')
            // false and (true or false) = false and true = false
            var result = JSONPredicate.Evaluate("client.address.postcode eq `wrong` and (client.address.number eq 123 or client.name eq `Jane`)", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_NestedParentheses_ShouldReturnTrue()
        {
            // (postcode eq 'e113et' and (number eq 123 or name eq 'Wrong')) or status eq 'inactive'
            // (true and (true or false)) or false = (true and true) or false = true or false = true
            var result = JSONPredicate.Evaluate("(client.address.postcode eq `e113et` and (client.address.number eq 123 or client.name eq `Wrong`)) or status eq `inactive`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_MultipleAndOperators_AllTrue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode eq `e113et` and client.address.number eq 123 and client.address.active eq true", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_MultipleOrOperators_OneTrue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode eq `wrong` or client.address.number eq 999 or client.address.active eq true", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_ComplexMixedExpression_ShouldReturnTrue()
        {
            // (postcode eq 'e113et' or postcode eq 'wrong') and (role in (admin, manager) or status eq 'active')
            // (true or false) and (true or true) = true and true = true
            var result = JSONPredicate.Evaluate("(client.address.postcode eq `e113et` or client.address.postcode eq `wrong`) and (client.role in (`admin`, `manager`) or status eq `active`)", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InOperatorWithAnd_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.tags in (`vip`, `premium`) and client.address.active eq true", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_NotOperatorWithOr_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.address.postcode not `wrong` or client.address.number eq 999", _testObject);
            Assert.That(result, Is.True);
        }

        // Numeric comparison tests
        [Test]
        public void Evaluate_GreaterThanOperator_Integer_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("age gt 25", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_GreaterThanOperator_Integer_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("age gt 35", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_GreaterThanOperator_Double_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("score gt 90.0", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_GreaterThanEqualOperator_EqualValue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("age gte 30", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_GreaterThanEqualOperator_GreaterValue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("age gte 25", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_GreaterThanEqualOperator_LessValue_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("age gte 35", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_LessThanOperator_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("age lt 35", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_LessThanOperator_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("age lt 25", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_LessThanEqualOperator_EqualValue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("age lte 30", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_LessThanEqualOperator_LessValue_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("age lte 35", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_LessThanEqualOperator_GreaterValue_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("age lte 25", _testObject);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_DoubleComparisons_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("price lt 30.0 and price gt 25.0", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_MixedNumericOperators_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("age gte 18 and score gt 90 and price lte 50", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_NumericWithOrOperator_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("age lt 25 or score gt 90", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_ComplexNumericExpression_ShouldReturnTrue()
        {
            // (age between 25 and 35) or (score > 90 and price < 35)
            var result = JSONPredicate.Evaluate("(age gte 25 and age lte 35) or (score gt 90 and price lt 35)", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_NegativeNumbers_ShouldReturnTrue()
        {
            var obj = new { temperature = -5 };
            var result = JSONPredicate.Evaluate("temperature lt 0", obj);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_ZeroComparisons_ShouldReturnTrue()
        {
            var obj = new { count = 0 };
            var result = JSONPredicate.Evaluate("count gte 0 and count lte 0", obj);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_StringNumericComparison_ShouldReturnTrue()
        {
            var obj = new { version = "2.5" };
            var result = JSONPredicate.Evaluate("version gt `2.0`", obj);
            Assert.That(result, Is.True);
        }

        // DateTime tests
        [Test]
        public void Evaluate_DateTimeGreaterThan_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt gt `2023-01-01T00:00:00Z`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_DateTimeLessThan_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt lt `2024-01-01T00:00:00Z`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_DateTimeGreaterThanOrEqual_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt gte `2023-01-15T10:30:00Z`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_DateTimeLessThanOrEqual_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt lte `2023-01-15T10:30:00Z`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_DateTimeEqual_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt eq `2023-01-15T10:30:00Z`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_StringDateTimeComparison_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("lastLogin gt `2023-11-01T00:00:00Z`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_DateOnlyFormat_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt gt `2023-01-01`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_DateTimeWithLogicalOperators_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt gt `2023-01-01` and age gte 18", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_DateTimeOrLogicalOperator_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt lt `2020-01-01` or age gte 25", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_ComplexDateTimeExpression_ShouldReturnTrue()
        {
            // (createdAt after 2023-01-01 and before 2024-01-01) or (expiryDate after 2024-01-01)
            var result = JSONPredicate.Evaluate("(createdAt gt `2023-01-01` and createdAt lt `2024-01-01`) or expiryDate gt `2024-01-01`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_DateTimeNotEqual_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt not `2020-01-01T00:00:00Z`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_DateTimeInFuture_ShouldReturnTrue()
        {
            var obj = new { futureDate = DateTime.UtcNow.AddDays(1) };
            var result = JSONPredicate.Evaluate("futureDate gt `2023-01-01T00:00:00Z`", obj);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_MultipleDateTimeComparisons_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt gte `2023-01-01` and lastLogin gte `2023-11-01` and expiryDate gt `2024-01-01`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_DateTimeWithParentheses_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("createdAt gt `2023-01-01` and (lastLogin gt `2023-11-01` or age gte 25)", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InvalidDateTimeFormat_ShouldThrowException()
        {
            var obj = new { date = DateTime.Parse("2023-01-15T10:30:00Z") };
            var ex = Assert.Throws<FormatException>(() =>
                JSONPredicate.Evaluate("date gt `2023-13-45`", obj));
            Assert.That(ex.Message, Does.Contain("Unable to parse DateTime value"));
        }

        [Test]
        public void Evaluate_DateTimeDifferentTimezones_ShouldReturnTrue()
        {
            var obj = new { utcDate = DateTime.Parse("2023-01-15T10:30:00Z") };
            var result = JSONPredicate.Evaluate("utcDate eq `2023-01-15T05:30:00-05:00`", obj); // Same time, different timezone
            Assert.That(result, Is.True);
        }

        // IN operator edge cases
        [Test]
        public void Evaluate_InOperatorWithSpaces_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.role in ( `admin` , `manager` , `user` )", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InOperatorWithMixedQuotes_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.name in (`John`, \"Jane\", 'Bob')", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InOperatorWithNumbers_ShouldReturnTrue()
        {
            var obj = new { category = 1 };
            var result = JSONPredicate.Evaluate("category in (1, 2, 3)", obj);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InOperatorWithBooleans_ShouldReturnTrue()
        {
            var obj = new { isActive = true };
            var result = JSONPredicate.Evaluate("isActive in (true, false)", obj);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InOperatorEmptyList_ShouldReturnFalse()
        {
            var result = JSONPredicate.Evaluate("client.role in ()", _testObject);
            Assert.That(result, Is.False);
        }

        // Edge case tests
        [Test]
        public void Evaluate_NullProperty_ShouldReturnFalse()
        {
            var obj = new { nullableValue = (string)null };
            var result = JSONPredicate.Evaluate("nullableValue eq `test`", obj);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Evaluate_EmptyExpression_ShouldThrowException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                JSONPredicate.Evaluate("", _testObject));
            Assert.That(ex.Message, Does.Contain("Invalid expression format"));
        }

        [Test]
        public void Evaluate_WhitespaceOnlyExpression_ShouldThrowException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                JSONPredicate.Evaluate("   ", _testObject));
            Assert.That(ex.Message, Does.Contain("Invalid expression format"));
        }

        [TestCase("client.address.postcode", "e113et", true)]
        [TestCase("client.address.postcode", "wrong", false)]
        [TestCase("client.address.number", "123", true)]
        [TestCase("client.address.number", "999", false)]
        public void Evaluate_ParameterizedTests(string path, string value, bool expected)
        {
            var expression = $"{path} eq `{value}`";
            var result = JSONPredicate.Evaluate(expression, _testObject);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Evaluate_ComplexInWithLogicalOperators_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("client.role in (`admin`, `manager`) and age gte 18 or status eq `pending`", _testObject);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Evaluate_InOperatorWithParenthesesAndNestedLogic_ShouldReturnTrue()
        {
            var result = JSONPredicate.Evaluate("(client.role in (`admin`, `manager`) or client.tags in (`vip`)) and client.address.active eq true", _testObject);
            Assert.That(result, Is.True);
        }
    }
}
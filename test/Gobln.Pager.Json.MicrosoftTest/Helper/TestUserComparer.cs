using Gobln.Pager.Json.MicrosoftTest.Model;
using System.Reflection;

namespace Gobln.Pager.Json.MicrosoftTest.Helper
{
    internal class TestUserComparer : IEqualityComparer<TestUser>
    {
        public bool Equals(TestUser expected, TestUser actual)
        {
            var props = typeof(TestUser).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var expectedValue = prop.GetValue(expected, null);
                var actualValue = prop.GetValue(actual, null);

                if (!expectedValue.Equals(actualValue))
                {
                    //throw new EqualException($"A value of \"{expectedValue}\" for property \"{prop.Name}\"", $"A value of \"{actualValue}\" for property \"{prop.Name}\"");
                    throw new Exception($"A value of \"{expectedValue}\" for property \"{prop.Name}\"");
                }
            }

            return true;
        }

        public int GetHashCode(TestUser parameterValue)
        {
            return Tuple.Create(parameterValue).GetHashCode();
        }
    }
}
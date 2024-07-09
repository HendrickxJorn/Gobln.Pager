using Gobln.Pager.Json.NewtonsoftTest.Model;
using System.Reflection;

namespace Gobln.Pager.Json.NewtonsoftTest.Helper
{
    internal class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User expected, User actual)
        {
            var props = typeof(User).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
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

        public int GetHashCode(User parameterValue)
        {
            return Tuple.Create(parameterValue).GetHashCode();
        }
    }
}
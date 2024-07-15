using Gobln.Pager.Json.Microsoft;
using Gobln.Pager.TestHelper.Comparer;
using Gobln.Pager.TestHelper.Data;
using Gobln.Pager.TestHelper.Model;
using System.Text.Json;
using Xunit;

namespace Gobln.Pager.Json.MicrosoftTest
{
    public class PageJsonConverterTest2
    {
        // todo test

        /// <summary>
        /// Serialize default Page. Deserialize Page in class
        /// </summary>
        [Fact]
        public void Serialize_Default_Page_Deserialize_Page_01()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PageJsonConverter(),
                },
            };

            var data = UserData.UserList;

            var page = new List<User>().ToPagedList();

            var json = JsonSerializer.Serialize(page);

            var result = JsonSerializer.Deserialize<PagedList<User>>(json);

            Assert.Equal(page, result, new UserComparer());
        }
    }
}
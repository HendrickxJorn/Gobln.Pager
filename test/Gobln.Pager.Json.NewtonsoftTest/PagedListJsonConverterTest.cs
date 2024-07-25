using Gobln.Pager;
using Golbn.Pager.Json.Newtonsoft;
using Gobln.Pager.TestHelper.Data;
using Gobln.Pager.TestHelper.Comparer;
using Gobln.Pager.TestHelper.Model;
using Newtonsoft.Json;
using Xunit;

namespace Gobln.Pager.Json.NewtonsoftTest
{
    public class PagedListJsonConverterTest
    {
        // todo test

        /// <summary>
        /// Serialize default Page. Deserialize Page in class
        /// </summary>
        [Fact]
        public void Serialize_Default_Page_Deserialize_Page_01()
        {
            //var serializeOptions = new JsonSerializerOptions
            //{
            //    Converters = {
            //        new PagedListJsonConverter(),
            //    },
            //};

            var data = UserData.UserList;

            var page = new List<User>().ToPagedList();

            var json = JsonConvert.SerializeObject(page);

            var result = JsonConvert.DeserializeObject<PagedList<User>>(json);

            Assert.Equal(page, result, new UserComparer());
        }
    }
}
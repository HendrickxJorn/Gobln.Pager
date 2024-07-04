using Gobln.Pager;
using Golbn.Pager.Json.Newtonsoft;
using Gobln.Pager.Json.NewtonsoftTest.Data;
using Gobln.Pager.Json.NewtonsoftTest.Helper;
using Gobln.Pager.Json.NewtonsoftTest.Model;
using Newtonsoft.Json;
using Xunit;

namespace Gobln.Pager.Json.NewtonsoftTest
{
    public class JsonSerialization
    {
        [Fact]
        public void TestConverter01()
        {
            var serializeOptions = new JsonSerializerSettings
            {
                Converters = { new PageJsonConverter<TestUser>() },
            };

            var data = TestUserData.TestUserList;

            var page = data.ToPage();

            var result = JsonConvert.SerializeObject(page, serializeOptions);

            var resultDe = JsonConvert.DeserializeObject<Page<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }

        [Fact]
        public void TestConverter02()
        {
            var serializeOptions = new JsonSerializerSettings
            {
                Converters = { new PageJsonConverter() },
            };

            var data = TestUserData.TestUserList;

            var page = data.ToPage();


            var result = JsonConvert.SerializeObject(page, serializeOptions);

            var resultDe = JsonConvert.DeserializeObject<Page<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }

        [Fact]
        public void TestConverter03()
        {
            var serializeOptions = new JsonSerializerSettings
            {
                Converters = { new PageJsonConverter() },
            };

            var data = TestUserData.TestUserList;

            var pageInClass = new PageInClass()
            {
                User = data.ToPage()
            };

            var result = JsonConvert.SerializeObject(pageInClass, serializeOptions);

            var resultDe = JsonConvert.DeserializeObject<PageInClass>(result, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, resultDe.SomeNumber);
            Assert.Equal(pageInClass.User, resultDe.User, new TestUserComparer());
        }

        [Fact]
        public void TestConverter04()
        {
            var serializeOptions = new JsonSerializerSettings
            {
                Converters = { new PageJsonConverter() },
            };

            var data = TestUserData.TestUserList;

            Page<TestUser> page = null;

            var result = JsonConvert.SerializeObject(page, serializeOptions);

            var resultDe = JsonConvert.DeserializeObject<Page<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }

        [Fact]
        public void TestConverter05()
        {
            var serializeOptions = new JsonSerializerSettings
            {
                Converters = { new PageJsonConverter() },
            };

            var data = TestUserData.TestUserList;

            var pageInClass = new PageInClass()
            {
                User = null
            };

            var result = JsonConvert.SerializeObject(pageInClass, serializeOptions);

            var resultDe = JsonConvert.DeserializeObject<PageInClass>(result, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, resultDe.SomeNumber);
            Assert.Equal(pageInClass.User, resultDe.User, new TestUserComparer());
        }

        [Fact]
        public void TestConverter06()
        {
            var serializeOptions = new JsonSerializerSettings
            {
                Converters = { new PageJsonConverter() },
            };

            var data = TestUserData.TestUserList;

            IPage<TestUser> page = data.ToPage();

            var result = JsonConvert.SerializeObject(page, serializeOptions);

            var resultDe = JsonConvert.DeserializeObject<Page<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }
    }
}

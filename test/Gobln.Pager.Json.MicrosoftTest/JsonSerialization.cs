using Gobln.Pager;
using Gobln.Pager.Json.Microsoft;
using Gobln.Pager.Json.MicrosoftTest.Data;
using Gobln.Pager.Json.MicrosoftTest.Helper;
using Gobln.Pager.Json.MicrosoftTest.Model;
using System.Text.Json;
using Xunit;

namespace Gobln.Pager.Json.MicrosoftTest
{
    public class JsonSerialization
    {
        [Fact]
        public void TestConverter01()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { new PageJsonConverter<TestUser>() },
            };

            var data = TestUserData.TestUserList;

            var page = data.ToPage();

            var result = JsonSerializer.Serialize(page, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<Page<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }

        [Fact]
        public void TestConverter02()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { new PageJsonConverter() },
            };

            var data = TestUserData.TestUserList;

            var page = data.ToPage();

            var result = JsonSerializer.Serialize(page, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<Page<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }

        [Fact]
        public void TestConverter03()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { new PageJsonConverter() },
            };

            var data = TestUserData.TestUserList;

            var pageInClass = new PageInClass()
            {
                User = data.ToPage()
            };

            var result = JsonSerializer.Serialize(pageInClass, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<PageInClass>(result, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, resultDe.SomeNumber);
            Assert.Equal(pageInClass.User, resultDe.User, new TestUserComparer());
        }

        [Fact]
        public void TestConverter04()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { new PageJsonConverter() },
            };

            var data = TestUserData.TestUserList;

            Page<TestUser> page = null;

            var result = JsonSerializer.Serialize(page, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<Page<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }

        [Fact]
        public void TestConverter05()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { new PageJsonConverter() },
            };

            var data = TestUserData.TestUserList;

            var pageInClass = new PageInClass()
            {
                User = null
            };

            var result = JsonSerializer.Serialize(pageInClass, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<PageInClass>(result, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, resultDe.SomeNumber);
            Assert.Equal(pageInClass.User, resultDe.User, new TestUserComparer());
        }

        [Fact]
        public void TestConverter06()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { 
                    new IPageJsonConverter<TestUser>(),
                },
            };

            var data = TestUserData.TestUserList;

            IPage<TestUser> page = data.ToPage();

            var result = JsonSerializer.Serialize(page, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<IPage<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }

        [Fact]
        public void TestConverter07()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PageJsonConverter(),
                },
            };

            var data = TestUserData.TestUserList;

            IPage<TestUser> page = data.ToPage();

            var result = JsonSerializer.Serialize(page, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<IPage<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }

        [Fact]
        public void TestConverter08()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PageJsonConverter(),
                },
            };

            var data = TestUserData.TestUserList;

            var pageInClass = new PageInClassGenericType()
            {
                User = data.ToPage()
            };

            var result = JsonSerializer.Serialize(pageInClass, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<PageInClassGenericType>(result, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, resultDe.SomeNumber);
            Assert.Equal(pageInClass.User, resultDe.User, new TestUserComparer());
        }

        [Fact]
        public void TestConverter09()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PageJsonConverter<TestUser>(),
                    new IPageJsonConverter<TestUser>(),
                },
            };

            var data = TestUserData.TestUserList;

            IPage<TestUser> page = data.ToPage();

            var result = JsonSerializer.Serialize(page, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<Page<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }

        [Fact]
        public void TestConverter10()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PageJsonConverter<TestUser>(),
                    new IPageJsonConverter<TestUser>(),
                },
            };

            var data = TestUserData.TestUserList;

            var pageInClass = new PageInClassGenericType()
            {
                User = data.ToPage()
            };

            var result = JsonSerializer.Serialize(pageInClass, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<PageInClassGenericType>(result, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, resultDe.SomeNumber);
            Assert.Equal(pageInClass.User, resultDe.User, new TestUserComparer());
        }

        [Fact]
        public void TestConverter11()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PageJsonConverter(),
                    new IPageJsonConverter<TestUser>(),
                },
            };

            var data = TestUserData.TestUserList;

            var pageInClass = new PageInClassGenericType()
            {
                User = data.ToPage()
            };

            var result = JsonSerializer.Serialize(pageInClass, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<PageInClassGenericType>(result, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, resultDe.SomeNumber);
            Assert.Equal(pageInClass.User, resultDe.User, new TestUserComparer());
        }

        [Fact]
        public void TestConverter12()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PageJsonConverter(),
                },
            };

            var data = TestUserData.TestUserList;

            var page = new List<TestUser>().ToPage();

            var result = JsonSerializer.Serialize(page, serializeOptions);

            var resultDe = JsonSerializer.Deserialize<Page<TestUser>>(result, serializeOptions);

            Assert.Equal(page, resultDe, new TestUserComparer());
        }
    }
}
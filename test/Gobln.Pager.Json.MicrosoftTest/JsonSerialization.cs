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
                Converters = { new PageJsonConverter<User>() },
            };

            var data = UserData.TestUserList;

            var page = data.ToPage();

            var json = JsonSerializer.Serialize(page, serializeOptions);

            var result = JsonSerializer.Deserialize<Page<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }

        [Fact]
        public void TestConverter02()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { new PageJsonConverter() },
            };

            var data = UserData.TestUserList;

            var page = data.ToPage();

            var json = JsonSerializer.Serialize(page, serializeOptions);

            var result = JsonSerializer.Deserialize<Page<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }

        [Fact]
        public void TestConverter03()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { new PageJsonConverter() },
            };

            var data = UserData.TestUserList;

            var pageInClass = new PageInClass()
            {
                User = data.ToPage()
            };

            var json = JsonSerializer.Serialize(pageInClass, serializeOptions);

            var result = JsonSerializer.Deserialize<PageInClass>(json, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, result.SomeNumber);
            Assert.Equal(pageInClass.User, result.User, new UserComparer());
        }

        [Fact]
        public void TestConverter04()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { new PageJsonConverter() },
            };

            var data = UserData.TestUserList;

            Page<User> page = null;

            var json = JsonSerializer.Serialize(page, serializeOptions);

            var result = JsonSerializer.Deserialize<Page<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }

        [Fact]
        public void TestConverter05()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { new PageJsonConverter() },
            };

            var data = UserData.TestUserList;

            var pageInClass = new PageInClass()
            {
                User = null
            };

            var json = JsonSerializer.Serialize(pageInClass, serializeOptions);

            var result = JsonSerializer.Deserialize<PageInClass>(json, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, result.SomeNumber);
            Assert.Equal(pageInClass.User, result.User, new UserComparer());
        }

        [Fact]
        public void TestConverter06()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = { 
                    new IPageJsonConverter<User>(),
                },
            };

            var data = UserData.TestUserList;

            IPage<User> page = data.ToPage();

            var json = JsonSerializer.Serialize(page, serializeOptions);

            var result = JsonSerializer.Deserialize<IPage<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
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

            var data = UserData.TestUserList;

            IPage<User> page = data.ToPage();

            var json = JsonSerializer.Serialize(page, serializeOptions);

            Assert.Throws<System.NotSupportedException>(() => JsonSerializer.Deserialize<IPage<User>>(json, serializeOptions));

            //var result = JsonSerializer.Deserialize<IPage<TestUser>>(json, serializeOptions);

            //Assert.Equal(page, result, new TestUserComparer());
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

            var data = UserData.TestUserList;

            var pageInClass = new PageInClassGenericType()
            {
                User = data.ToPage()
            };

            var json = JsonSerializer.Serialize(pageInClass, serializeOptions);

            Assert.Throws<System.NotSupportedException>(() => JsonSerializer.Deserialize<PageInClassGenericType>(json, serializeOptions));

            //var result = JsonSerializer.Deserialize<PageInClassGenericType>(json, serializeOptions);

            //Assert.Equal(pageInClass.SomeNumber, result.SomeNumber);
            //Assert.Equal(pageInClass.User, result.User, new TestUserComparer());
        }

        [Fact]
        public void TestConverter09()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PageJsonConverter<User>(),
                    new IPageJsonConverter<User>(),
                },
            };

            var data = UserData.TestUserList;

            IPage<User> page = data.ToPage();

            var json = JsonSerializer.Serialize(page, serializeOptions);

            var result = JsonSerializer.Deserialize<Page<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }

        [Fact]
        public void TestConverter10()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PageJsonConverter<User>(),
                    new IPageJsonConverter<User>(),
                },
            };

            var data = UserData.TestUserList;

            var pageInClass = new PageInClassGenericType()
            {
                User = data.ToPage()
            };

            var json = JsonSerializer.Serialize(pageInClass, serializeOptions);

            var result = JsonSerializer.Deserialize<PageInClassGenericType>(json, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, result.SomeNumber);
            Assert.Equal(pageInClass.User, result.User, new UserComparer());
        }

        [Fact]
        public void TestConverter11()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PageJsonConverter(),
                    new IPageJsonConverter<User>(),
                },
            };

            var data = UserData.TestUserList;

            var pageInClass = new PageInClassGenericType()
            {
                User = data.ToPage()
            };

            var json = JsonSerializer.Serialize(pageInClass, serializeOptions);

            var result = JsonSerializer.Deserialize<PageInClassGenericType>(json, serializeOptions);

            Assert.Equal(pageInClass.SomeNumber, result.SomeNumber);
            Assert.Equal(pageInClass.User, result.User, new UserComparer());
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

            var data = UserData.TestUserList;

            var page = new List<User>().ToPage();

            var json = JsonSerializer.Serialize(page, serializeOptions);

            var result = JsonSerializer.Deserialize<Page<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }
    }
}
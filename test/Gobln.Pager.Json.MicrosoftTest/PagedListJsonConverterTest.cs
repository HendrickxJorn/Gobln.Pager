using Gobln.Pager.Json.Microsoft;
using Gobln.Pager.TestHelper.Comparer;
using Gobln.Pager.TestHelper.Data;
using Gobln.Pager.TestHelper.EFHelper;
using Gobln.Pager.TestHelper.Model;
using System.Text.Json;
using Xunit;

namespace Gobln.Pager.Json.MicrosoftTest
{
    public class PagedListJsonConverterTest
    {
        // todo test

        /// <summary>
        /// Serialize default Page. Deserialize Page in class
        /// </summary>
        [Fact]
        public void Default_Serialize_PagedList_Default_Deserialize_PagedList_01()
        {
            var data = UserData.UserList;

            var page = new List<User>().ToPagedList();

            var json = JsonSerializer.Serialize(page);

            var result = JsonSerializer.Deserialize<PagedList<User>>(json);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default Page. Deserialize Page in class
        /// </summary>
        [Fact]
        public void Default_Serialize_PagedList_Default_Deserialize_IPagedList_01()
        {
            var data = UserData.UserList;

            var page = new List<User>().ToPagedList();

            var json = JsonSerializer.Serialize(page);

            // Can not Deserialize with the default System.Text.Json
            Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<IPagedList<User>>(json));

            //var result = JsonSerializer.Deserialize<IPagedList<User>>(json);

            //Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default Page. Deserialize Page in class
        /// </summary>
        [Fact]
        public void Default_Serialize_Empty_PagedList_Default_Deserialize_PagedList_01()
        {
            var data = UserData.UserList;

            var page = new PagedList<User>();

            var json = JsonSerializer.Serialize(page);

            var result = JsonSerializer.Deserialize<PagedList<User>>(json);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default Page. Deserialize Page in class
        /// </summary>
        [Fact]
        public void Default_Serialize_Empty_PagedList_Default_Deserialize_IPagedList_01()
        {
            var data = UserData.UserList;

            var page = new PagedList<User>();

            var json = JsonSerializer.Serialize(page);

            // Can not Deserialize with the default System.Text.Json
            Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<IPagedList<User>>(json));

            //var result = JsonSerializer.Deserialize<IPagedList<User>>(json);

            //Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default Page. Deserialize Page in class
        /// </summary>
        [Fact]
        public void Serialize_Empty_PagedList_Deserialize_PagedList_01()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PagedListJsonConverter(),
                },
            };

            var data = UserData.UserList;

            var page = new List<User>().ToPagedList();

            var json = JsonSerializer.Serialize(page);

            var result = JsonSerializer.Deserialize<PagedList<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default Page. Deserialize Page in class
        /// </summary>
        [Fact]
        public void Serialize_Empty_PagedList_Deserialize_PagedList_02()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PagedListJsonConverter<User>(),
                },
            };

            var data = UserData.UserList;

            var page = new List<User>().ToPagedList();

            var json = JsonSerializer.Serialize(page);

            var result = JsonSerializer.Deserialize<PagedList<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize IPage in class. Deserialize IPage in class
        /// </summary>
        [Fact]
        public void Serialize_Empty_PagedList_Deserialize_IPagedList_01()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PagedListJsonConverter(),
                },
            };

            var data = UserData.UserList;

            var page = new List<User>().ToPagedList();

            var json = JsonSerializer.Serialize(page);

            var result = JsonSerializer.Deserialize<IPagedList<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize IPage in class. Deserialize IPage in class
        /// </summary>
        [Fact]
        public void Serialize_Empty_PagedList_Deserialize_IPagedList_02()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                Converters = {
                    new PagedListJsonConverter<User>(),
                    new IPagedListJsonConverter<User>(),
                },
            };

            var data = UserData.UserList;

            var page = new List<User>().ToPagedList();

            var json = JsonSerializer.Serialize(page);

            var result = JsonSerializer.Deserialize<IPagedList<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }
    }
}
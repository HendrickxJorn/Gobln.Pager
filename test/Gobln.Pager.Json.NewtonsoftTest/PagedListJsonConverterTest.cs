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
        /// <summary>
        /// Serialize default PagedList. Deserialize PagedList. With no options
        /// </summary>
        [Fact]
        public void NoOptions_Serialize_PagedList_Default_Deserialize_PagedList_01()
        {
            var data = UserData.UserList;

            var page = data.ToPagedList();

            var json = JsonConvert.SerializeObject(page);

            var result = JsonConvert.DeserializeObject<PagedList<User>>(json);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default PagedList. Deserialize IPagedList. With no options
        /// 
        /// Will give an Exception
        /// 
        /// </summary>
        [Fact]
        public void NoOptions_Serialize_PagedList_Default_Deserialize_IPagedList_01()
        {
            var data = UserData.UserList;

            var page = data.ToPagedList();

            var json = JsonConvert.SerializeObject(page);

            // Can not Deserialize with the System.Text.Json
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<IPagedList<User>>(json));

            //var result = JsonConvert.DeserializeObject<IPagedList<User>>(json);

            //Assert.Equal(data, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default PagedList. Deserialize PagedList. With no options
        /// </summary>
        [Fact]
        public void NoOptions_Serialize_Empty_PagedList_Default_Deserialize_PagedList_01()
        {
            var page = new PagedList<User>();

            var json = JsonConvert.SerializeObject(page);

            var result = JsonConvert.DeserializeObject<PagedList<User>>(json);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default PagedList. Deserialize PagedList. With no options
        /// </summary>
        [Fact]
        public void NoOptions_Serialize_NULL_PagedList_Default_Deserialize_IPagedList_01()
        {
            PagedList<User> page = null;

            var json = JsonConvert.SerializeObject(page);

            var result = JsonConvert.DeserializeObject<IPagedList<User>>(json);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default PagedList. Deserialize PagedList. With no options
        /// </summary>
        [Fact]
        public void NoOptions_Serialize_NULL_PagedList_Default_Deserialize_PagedList_01()
        {
            PagedList<User> page = null;

            var json = JsonConvert.SerializeObject(page);

            var result = JsonConvert.DeserializeObject<PagedList<User>>(json);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default PagedList. Deserialize PagedList. With no options
        /// 
        /// Will give an Exception
        /// 
        /// </summary>
        [Fact]
        public void NoOptions_Serialize_Empty_PagedList_Default_Deserialize_IPagedList_01()
        {
            var page = new PagedList<User>();

            var json = JsonConvert.SerializeObject(page);

            // Can not Deserialize with the System.Text.Json
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<IPagedList<User>>(json));

            //var result = JsonConvert.DeserializeObject<IPagedList<User>>(json);

            //Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default PagedList. Deserialize PagedList.
        /// </summary>
        [Fact]
        public void Serialize_Empty_PagedList_Deserialize_PagedList_01()
        {
            var serializeOptions = new JsonSerializerSettings
            {
                Converters = {
                    new PagedListJsonConverter(),
                },
            };

            var page = new List<User>().ToPagedList();

            var json = JsonConvert.SerializeObject(page);

            var result = JsonConvert.DeserializeObject<PagedList<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize IPagedList. Deserialize IPagedList.
        /// </summary>
        [Fact]
        public void Serialize_Empty_PagedList_Deserialize_IPagedList_01()
        {
            var serializeOptions = new JsonSerializerSettings
            {
                Converters = {
                    new PagedListJsonConverter(),
                },
            };

            var page = new List<User>().ToPagedList();

            var json = JsonConvert.SerializeObject(page);

            var result = JsonConvert.DeserializeObject<IPagedList<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize default PagedList. Deserialize PagedList.
        /// </summary>
        [Fact]
        public void Serialize_PagedList_Deserialize_PagedList_01()
        {
            var serializeOptions = new JsonSerializerSettings
            {
                Converters = {
                    new PagedListJsonConverter(),
                },
            };

            var data = UserData.UserList;

            var page = data.ToPagedList();

            var json = JsonConvert.SerializeObject(page);

            var result = JsonConvert.DeserializeObject<PagedList<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }

        /// <summary>
        /// Serialize IPagedList. Deserialize IPagedList.
        /// </summary>
        [Fact]
        public void Serialize_PagedList_Deserialize_IPagedList_01()
        {
            var serializeOptions = new JsonSerializerSettings
            {
                Converters = {
                    new PagedListJsonConverter(),
                },
            };

            var data = UserData.UserList;

            var page = data.ToPagedList();

            var json = JsonConvert.SerializeObject(page);

            var result = JsonConvert.DeserializeObject<IPagedList<User>>(json, serializeOptions);

            Assert.Equal(page, result, new UserComparer());
        }
    }
}
using Gobln.Pager.Json.Microsoft;
using Gobln.Pager.TestHelper.Comparer;
using Gobln.Pager.TestHelper.Data;
using Gobln.Pager.TestHelper.Model;
using System.Text.Json;
using Xunit;

namespace Gobln.Pager.Json.MicrosoftTest;

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

        var json = JsonSerializer.Serialize(page);

        var result = JsonSerializer.Deserialize<PagedList<User>>(json);

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

        var json = JsonSerializer.Serialize(page);

        // Can not Deserialize with the System.Text.Json
        Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<IPagedList<User>>(json));

        //var result = JsonSerializer.Deserialize<IPagedList<User>>(json);

        //Assert.Equal(data, result, new UserComparer());
    }

    /// <summary>
    /// Serialize default PagedList. Deserialize PagedList. With no options
    /// </summary>
    [Fact]
    public void NoOptions_Serialize_Empty_PagedList_Default_Deserialize_PagedList_01()
    {
        var page = new PagedList<User>();

        var json = JsonSerializer.Serialize(page);

        var result = JsonSerializer.Deserialize<PagedList<User>>(json);

        Assert.Equal(page, result, new UserComparer());
    }

    /// <summary>
    /// Serialize default PagedList. Deserialize PagedList. With no options
    /// </summary>
    [Fact]
    public void NoOptions_Serialize_NULL_PagedList_Default_Deserialize_IPagedList_01()
    {
        PagedList<User> page = null;

        var json = JsonSerializer.Serialize(page);

        var result = JsonSerializer.Deserialize<IPagedList<User>>(json);

        Assert.Equal(page, result, new UserComparer());
    }

    /// <summary>
    /// Serialize default PagedList. Deserialize PagedList. With no options
    /// </summary>
    [Fact]
    public void NoOptions_Serialize_NULL_PagedList_Default_Deserialize_PagedList_01()
    {
        PagedList<User> page = null;

        var json = JsonSerializer.Serialize(page);

        var result = JsonSerializer.Deserialize<PagedList<User>>(json);

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

        var json = JsonSerializer.Serialize(page);

        // Can not Deserialize with the System.Text.Json
        Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<IPagedList<User>>(json));

        //var result = JsonSerializer.Deserialize<IPagedList<User>>(json);

        //Assert.Equal(page, result, new UserComparer());
    }

    /// <summary>
    /// Serialize default PagedList. Deserialize PagedList.
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

        var page = new List<User>().ToPagedList();

        var json = JsonSerializer.Serialize(page);

        var result = JsonSerializer.Deserialize<PagedList<User>>(json, serializeOptions);

        Assert.Equal(page, result, new UserComparer());
    }

    /// <summary>
    /// Serialize IPagedList. Deserialize IPagedList.
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

        var page = new List<User>().ToPagedList();

        var json = JsonSerializer.Serialize(page);

        var result = JsonSerializer.Deserialize<IPagedList<User>>(json, serializeOptions);

        Assert.Equal(page, result, new UserComparer());
    }

    /// <summary>
    /// Serialize default PagedList. Deserialize PagedList.
    /// </summary>
    [Fact]
    public void Serialize_PagedList_Deserialize_PagedList_01()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters = {
                new PagedListJsonConverter(),
            },
        };

        var data = UserData.UserList;

        var page = data.ToPagedList();

        var json = JsonSerializer.Serialize(page);

        var result = JsonSerializer.Deserialize<PagedList<User>>(json, serializeOptions);

        Assert.Equal(page, result, new UserComparer());
    }

    /// <summary>
    /// Serialize IPagedList. Deserialize IPagedList.
    /// </summary>
    [Fact]
    public void Serialize_PagedList_Deserialize_IPagedList_01()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters = {
                new PagedListJsonConverter(),
            },
        };

        var data = UserData.UserList;

        var page = data.ToPagedList();

        var json = JsonSerializer.Serialize(page);

        var result = JsonSerializer.Deserialize<IPagedList<User>>(json, serializeOptions);

        Assert.Equal(page, result, new UserComparer());
    }
}
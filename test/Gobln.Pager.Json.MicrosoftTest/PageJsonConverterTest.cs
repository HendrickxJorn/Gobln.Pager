using Gobln.Pager.Json.Microsoft;
using Gobln.Pager.TestHelper.Comparer;
using Gobln.Pager.TestHelper.Data;
using Gobln.Pager.TestHelper.Model;
using System.Text.Json;
using Xunit;

namespace Gobln.Pager.Json.MicrosoftTest;

public class PageJsonConverterTest
{
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

        var page = new List<User>().ToPage();

        var json = JsonSerializer.Serialize(page, serializeOptions);

        var result = JsonSerializer.Deserialize<Page<User>>(json, serializeOptions);

        Assert.Equal(page, result, new UserComparer());
    }

    /// <summary>
    /// Serialize Empty Page in class. Deserialize Page in class
    /// </summary>
    [Fact]
    public void Serialize_Empty_Page_Class_Deserialize_Empty_Page_Class_01()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters = {
                new PageJsonConverter()
            },
        };

        var data = UserData.UserList;

        var pageInClass = new PageInClass()
        {
            User = null
        };

        var json = JsonSerializer.Serialize(pageInClass, serializeOptions);

        var result = JsonSerializer.Deserialize<PageInClass>(json, serializeOptions);

        Assert.Equal(pageInClass.SomeNumber, result.SomeNumber);
        Assert.Equal(pageInClass.User, result.User, new UserComparer());
    }

    /// <summary>
    /// Serialize Page in class. Deserialize Page in class
    /// </summary>
    [Fact]
    public void Serialize_Empty_Page_Deserialize_Empty_Page_01()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters = {
                new PageJsonConverter()
            },
        };

        var data = UserData.UserList;

        Page<User> page = null;

        var json = JsonSerializer.Serialize(page, serializeOptions);

        var result = JsonSerializer.Deserialize<Page<User>>(json, serializeOptions);

        Assert.Equal(page, result, new UserComparer());
    }

    /// <summary>
    /// Serialize IPage in class. Deserialize IPage in class
    /// </summary>
    [Fact]
    public void Serialize_IPage_Class_Deserialize_IPage_Class_01()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters = {
                new PageJsonConverter(),
            },
        };

        var data = UserData.UserList;

        var pageInClass = new PageInClassGenericType()
        {
            User = data.ToPage()
        };

        var json = JsonSerializer.Serialize(pageInClass, serializeOptions);

        var result = JsonSerializer.Deserialize<PageInClassGenericType>(json, serializeOptions);

        Assert.Equal(pageInClass.SomeNumber, result.SomeNumber);
        Assert.Equal(pageInClass.User, result.User, new UserComparer());
    }

    /// <summary>
    /// Serialize IPage. Deserialize to IPage
    /// </summary>
    [Fact]
    public void Serialize_IPage_Deserialize_IPage_01()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters = {
                new PageJsonConverter(),
            },
        };

        var data = UserData.UserList;

        IPage<User> page = data.ToPage();

        var json = JsonSerializer.Serialize(page, serializeOptions);

        var result = JsonSerializer.Deserialize<IPage<User>>(json, serializeOptions);

        Assert.Equal(page, result, new UserComparer());
    }

    /// <summary>
    /// Serialize IPage. Deserialize to Page
    /// </summary>
    [Fact]
    public void Serialize_IPage_Deserialize_Page_01()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters = {
                 new PageJsonConverter(),
            },
        };

        var data = UserData.UserList;

        IPage<User> page = data.ToPage();

        var json = JsonSerializer.Serialize(page, serializeOptions);

        var result = JsonSerializer.Deserialize<Page<User>>(json, serializeOptions);

        Assert.Equal(page, result, new UserComparer());
    }

    /// <summary>
    /// Serialize Page in class. Deserialize Page in class
    /// </summary>
    [Fact]
    public void Serialize_Page_Class_Deserialize_Page_Class_01()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters = {
                new PageJsonConverter()
            },
        };

        var data = UserData.UserList;

        var pageInClass = new PageInClass()
        {
            User = data.ToPage()
        };

        var json = JsonSerializer.Serialize(pageInClass, serializeOptions);

        var result = JsonSerializer.Deserialize<PageInClass>(json, serializeOptions);

        Assert.Equal(pageInClass.SomeNumber, result.SomeNumber);
        Assert.Equal(pageInClass.User, result.User, new UserComparer());
    }

    /// <summary>
    /// Serialize Page in class (null). Deserialize Page in class
    /// </summary>
    [Fact]
    public void Serialize_Page_Class_Null_Deserialize_Empty_Page_Class_01()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters = {
                new PageJsonConverter()
            },
        };

        var data = UserData.UserList;

        PageInClass pageInClass = null;

        var json = JsonSerializer.Serialize(pageInClass, serializeOptions);

        var result = JsonSerializer.Deserialize<PageInClass>(json, serializeOptions);

        Assert.Equal(pageInClass, result);
    }

    /// <summary>
    /// Serialize Page. Deserialize to Page
    /// </summary>
    [Fact]
    public void Serialize_Page_Deserialize_Page_01()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters = {
                new PageJsonConverter()
            },
        };

        var data = UserData.UserList;

        var page = data.ToPage();

        var json = JsonSerializer.Serialize(page, serializeOptions);

        var result = JsonSerializer.Deserialize<Page<User>>(json, serializeOptions);

        Assert.Equal(page, result, new UserComparer());
    }
}
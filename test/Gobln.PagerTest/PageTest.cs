using Gobln.Pager;
using Gobln.Pager.TestHelper.Comparer;
using Gobln.Pager.TestHelper.Data;
using Gobln.Pager.TestHelper.EFHelper;
using Gobln.Pager.TestHelper.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Gobln.PagerTest;

public class PageTest
{
    /// <summary>
    /// Create a Page from a List
    /// </summary>
    [Fact]
    public void Create_Page_From_IEnumerable_01()
    {
        var data = UserData.UserList;

        // Create an default page
        // Will put everything on 1 page
        var result = data.ToPage();

        var expected = data;

        Assert.Equal(1, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create a Page from a List
    /// </summary>
    [Fact]
    public void Create_Page_From_IEnumerable_02()
    {
        var data = UserData.UserList;

        // Create an default page
        // Will put everything on 1 page
        var result = new Page<User>(data);

        var expected = data;

        Assert.Equal(1, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create a Page from a List
    /// </summary>
    [Fact]
    public void Create_Page_From_IEnumerable_03()
    {
        var data = UserData.UserList;

        // Create an default page
        // Will put everything on 1 page
        var result = Page.FromEnumerable(data);

        var expected = data;

        Assert.Equal(1, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create a Page from a PagedList
    /// </summary>
    [Fact]
    public void Create_Page_From_PagedList_01()
    {
        var data = UserData.UserList.ToPagedList();

        // Create an default page
        // Will put everything one 1 page
        var result = IEnumerableExtensions.ToPage(data);

        var expected = data;

        Assert.Equal(1, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create a Page from a List, but overreach
    /// </summary>
    [Fact]
    public void Create_Page_From_IEnumerable_Overreach_01()
    {
        var data = UserData.UserList;

        // Ask for page 5 at 15 items per page. The list holds 29 items, so 2 pages.
        // A page past the end gives back the last page: items 16 to 29
        var result = data.ToPage(5, 15);

        var expected = data.Skip(15).ToList();

        Assert.Equal(2, result.PageIndex);
        Assert.Equal(2, result.TotalPageCount);
        Assert.Equal(29, result.TotalItemCount);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create a Page from an IQueryable, but overreach
    /// </summary>
    [Fact]
    public void Create_Page_From_IQueryable_Overreach_01()
    {
        var data = UserData.UserList.AsQueryable();

        // Same as the IEnumerable case: page 5 of 2 gives back the last page
        var result = data.ToPage(5, 15);

        var expected = data.Skip(15).ToList();

        Assert.Equal(2, result.PageIndex);
        Assert.Equal(2, result.TotalPageCount);
        Assert.Equal(29, result.TotalItemCount);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create a Page from a pre-paged list that came back empty
    /// </summary>
    [Fact]
    public void Create_Page_From_Prepaged_Empty_01()
    {
        // The source holds 30 items, but none came back for page 4 (past the end).
        // The page keeps the totals, so a pager still knows there are 3 pages.
        var result = new List<User>().ToPage(4, 10, 30);

        Assert.Equal(3, result.PageIndex);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPageCount);
        Assert.Equal(30, result.TotalItemCount);
        Assert.Empty(result);
    }

    /// <summary>
    /// Create a Page from an pre-paged list
    /// </summary>
    [Fact]
    public void Create_Page_From_Prepaged_IEnumerable_01()
    {
        var data = UserData.UserList;

        // Create a Page for 15 items on page 5, with an total of 200 items
        var result = data.ToPage(5, 15, 200);

        var expected = data;

        Assert.Equal(5, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Make use of PageFilter
    /// </summary>
    [Fact]
    public void Create_Page_With_PageFilter_01()
    {
        var data = UserData.UserList;

        // Set the page size on 2 and the index on 5
        var filterModel = new PageFilter(5, 2);

        var result = data.ToPage(filterModel);

        // Take the 8the and 9the item
        var expected = data.Skip(8).Take(2).ToList();

        Assert.Equal(5, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Make use of PageFilter
    /// </summary>
    [Fact]
    public void Create_Page_With_PageFilter_PrePaged_01()
    {
        var data = UserData.UserList;

        var totalItemCount = data.Count();

        var smallDataSet = data.Skip(8).Take(2).ToList();

        // Set the page size on 2 and the index on 5
        var filterModel = new PageFilter(5, 2);

        var result = smallDataSet.ToPage(filterModel, totalItemCount);

        // Take the 8the and 9the item
        var expected = smallDataSet;

        Assert.Equal(5, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create a Page from a empty List
    /// </summary>
    [Fact]
    public void Create_Empty_Page_01()
    {
        var result = new List<User>().ToPage();

        var expected = new List<User>().ToArray();

        Assert.Equal(1, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create a Page from an pre-paged list
    /// </summary>
    [Fact]
    public void Create_Page_With_Prepaged_Zero_01()
    {
        var data = UserData.UserList;

        // Create a Page for 15 items on page 5, with an total of 0 items
        // will return empty Page
        var result = data.ToPage(5, 15, 0);

        var expected = new List<User>();

        Assert.Equal(1, result.PageIndex);
        Assert.Equal(0, result.PageSize);
        Assert.Equal(1, result.TotalPageCount);
        Assert.Equal(0, result.TotalItemCount);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create a Page from an pre-paged list
    /// </summary>
    [Fact]
    public void Create_Page_With_Prepaged_Zero_02()
    {
        var data = UserData.UserList;

        // Create a Page for 15 items on page 5, with an total of 0 items
        // will return empty Page
        var result = new Page<User>(data, 5, 15, 0);

        var expected = new List<User>();

        Assert.Equal(1, result.PageIndex);
        Assert.Equal(0, result.PageSize);
        Assert.Equal(1, result.TotalPageCount);
        Assert.Equal(0, result.TotalItemCount);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Use the FromEnumerable
    /// </summary>
    [Fact]
    public void Create_Page_With_FromEnumerable_01()
    {
        var data = UserData.UserList;

        var result = Page.FromEnumerable(data, 2, 15);

        var expected = data;

        Assert.Equal(2, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Use the FromEnumerable with PrePaged data
    /// </summary>
    [Fact]
    public void Create_Page_With_FromEnumerablePrepaged_01()
    {
        var data = UserData.UserList;

        var result = Page.FromEnumerable(data, 2, 15, 30);

        var expected = data;

        Assert.Equal(2, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create page form EF
    /// </summary>
    [Fact]
    public void Create_Page_From_EF01()
    {
        /// mock EF
        var data = UserData.UserList
            .AsQueryable();

        var mockSet = new Mock<DbSet<User>>();
        mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator);

        var mockContext = new Mock<UserContext>();
        mockContext.Setup(c => c.Users).Returns(mockSet.Object);

        /// Call Service with mock EF
        var service = new UserService(mockContext.Object);
        var result = service.GetPageUsers(new PageFilter(2, 5));

        var expected = data.Skip(5).Take(5).ToList();

        Assert.Equal(2, result.PageIndex);
        Assert.Equal(expected, result, new UserComparer());
    }
}
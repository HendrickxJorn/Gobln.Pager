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
        // Not asked for a page size, so there is none to keep
        Assert.Equal(0, result.PageSize);
        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// Create a Page from a empty List
    /// </summary>
    [Fact]
    public async Task Create_Empty_Page_02()
    {
        var result = new List<User>().ToPage(1, 10);

        var expected = new List<User>().ToArray();

        Assert.Equal(expected, result, new UserComparer());
        Assert.Equal(1, result.PageIndex);
        Assert.Equal(10, result.PageSize);
    }

    /// <summary>
    /// Create a Page without a page size from a empty List
    /// </summary>
    [Fact]
    public void Create_Empty_Page_03()
    {
        // A page size of 0 makes an empty page, as with a total item count of 0
        var result = new List<User>().ToPage(1, 0);

        var expected = new List<User>().ToArray();

        Assert.Equal(expected, result, new UserComparer());
        Assert.Equal(1, result.PageIndex);
        Assert.Equal(0, result.PageSize);
        Assert.Equal(0, result.TotalItemCount);
        Assert.Equal(1, result.TotalPageCount);
    }

    /// <summary>
    /// Create a Page without a page size from an empty pre-paged List
    /// </summary>
    [Fact]
    public void Create_Empty_Page_04()
    {
        // A page size of 0 is allowed when the total item count is 0
        var result = new List<User>().ToPage(1, 0, 0);

        var expected = new List<User>().ToArray();

        Assert.Equal(expected, result, new UserComparer());
        Assert.Equal(1, result.PageIndex);
        Assert.Equal(0, result.PageSize);
        Assert.Equal(0, result.TotalItemCount);
        Assert.Equal(1, result.TotalPageCount);
    }

    /// <summary>
    /// A page size of 0 throws when the pre-paged List has items in total
    /// </summary>
    [Fact]
    public void Create_Empty_Page_05()
    {
        // 20 items can't be put on pages of 0 items
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => UserData.UserList.ToPage(1, 0, 20));

        Assert.Equal("pageSize", ex.ParamName);
    }

    /// <summary>
    /// Create a Page without a page size from a List with items
    /// </summary>
    [Fact]
    public void Create_Empty_Page_06()
    {
        // A page size of 0 makes an empty page whatever the List holds: the total item count is set to 0
        var result = UserData.UserList.ToPage(1, 0);

        var expected = new List<User>().ToArray();

        Assert.Equal(expected, result, new UserComparer());
        Assert.Equal(1, result.PageIndex);
        Assert.Equal(0, result.PageSize);
        Assert.Equal(0, result.TotalItemCount);
        Assert.Equal(1, result.TotalPageCount);
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
        Assert.Equal(15, result.PageSize);
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
        Assert.Equal(15, result.PageSize);
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
    /// Page.FromEnumerable and the constructor count null as an empty collection
    /// </summary>
    [Fact]
    public void Create_Page_From_Null_01()
    {
        IEnumerable<User>? data = null;

        // Without a page size, as the whole of an empty collection
        var whole = Page.FromEnumerable(data!);

        Assert.Empty(whole);
        Assert.Equal(0, whole.PageSize);
        Assert.Equal(0, whole.TotalItemCount);

        // Without a total, as an empty collection that is counted: it keeps its page size
        var counted = Page.FromEnumerable(data!, 2, 10);

        Assert.Empty(counted);
        Assert.Equal(1, counted.PageIndex);
        Assert.Equal(10, counted.PageSize);
        Assert.Equal(0, counted.TotalItemCount);

        // With a total, as a pre-paged page that came back empty: it keeps its totals
        var prePaged = new[] { Page.FromEnumerable(data!, 2, 10, 25), new Page<User>(data!, 2, 10, 25) };

        Assert.All(prePaged, page =>
        {
            Assert.Empty(page);
            Assert.Equal(2, page.PageIndex);
            Assert.Equal(10, page.PageSize);
            Assert.Equal(25, page.TotalItemCount);
            Assert.Equal(3, page.TotalPageCount);
        });
    }

    /// <summary>
    /// Create a Page with a PageFilter, a total and an explicit prePaged flag
    /// </summary>
    [Fact]
    public void Create_Page_With_PageFilter_PrePaged_02()
    {
        var data = UserData.UserList.ToList();

        var filterModel = new PageFilter(2, 10);

        // prePaged: false takes the page out of the collection, prePaged: true takes the collection as the page
        var sliced = data.ToPage(filterModel, data.Count, prePaged: false);
        var prePaged = data.Skip(10).Take(10).ToList().ToPage(filterModel, data.Count, prePaged: true);

        var expected = data.Skip(10).Take(10).ToList();

        Assert.Equal(expected, sliced, new UserComparer());
        Assert.Equal(expected, prePaged, new UserComparer());
        Assert.Equal(2, sliced.PageIndex);
        Assert.Equal(3, prePaged.TotalPageCount);
    }

    /// <summary>
    /// Put a whole query on one Page
    /// </summary>
    [Fact]
    public void Create_Page_From_IQueryable_01()
    {
        var data = UserData.UserList.AsQueryable();

        var result = data.ToPage();

        Assert.Equal(data, result, new UserComparer());
        Assert.Equal(1, result.PageIndex);
        Assert.Equal(29, result.PageSize);
        Assert.Equal(29, result.TotalItemCount);
        Assert.Equal(1, result.TotalPageCount);

        // As ToPage() of an empty collection: not asked for a page size, so there is none
        var empty = new List<User>().AsQueryable().ToPage();

        Assert.Empty(empty);
        Assert.Equal(0, empty.PageSize);
        Assert.Equal(0, empty.TotalItemCount);
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
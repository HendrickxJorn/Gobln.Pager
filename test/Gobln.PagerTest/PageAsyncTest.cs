using Gobln.Pager;
using Gobln.Pager.TestHelper.Comparer;
using Gobln.Pager.TestHelper.Data;
using Gobln.Pager.TestHelper.EFHelper;
using Gobln.Pager.TestHelper.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Gobln.PagerTest
{
    public class PageAsyncTest
    {
        /// <summary>
        /// Create a Page from a List
        /// </summary>
        [Fact]
        public async Task Create_Page_From_IEnumerable()
        {
            var data = UserData.UserList;

            // Create an default page
            // Will put everything one 1 page
            var result = await data.ToPageAsync();

            var expected = data;

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Create a Page from a PagedList
        /// </summary>
        [Fact]
        public async Task Create_Page_From_PagedList()
        {
            var data = UserData.UserList.ToPagedList();

            // Create an default page
            // Will put everything one 1 page
            var result = await data.ToPageAsync();

            var expected = data;

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Create a Page from a PagedList, but overreach
        /// </summary>
        [Fact]
        public async Task Create_Page_From_IEnumerable_Overreach()
        {
            var data = UserData.UserList;

            // Take the items that would be on the 5 page
            // items 60 to 75
            // This is not possible, because the PagedList only contains 15 items
            // And will then give back an empty Page
            var result = await data.ToPageAsync(5, 15);

            var expected = new List<User>();

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Create a Page from an pre-paged list
        /// </summary>
        [Fact]
        public async Task Create_Page_From_Prepaged_IEnumerable()
        {
            var data = UserData.UserList;

            // Create a Page for 15 items on page 5, with an total of 200 items
            var result = await data.ToPageAsync(5, 15, 200);

            var expected = data;

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Make use of PageFilter
        /// </summary>
        [Fact]
        public async Task Create_Page_With_PageFilter()
        {
            var data = UserData.UserList;

            // Set the page size on 2 and the index on 5
            var filterModel = new PageFilter(5, 2);

            var result = await data.ToPageAsync(filterModel);

            // Take the 8the and 9the item
            var expected = data.Skip(8).Take(2).ToList();

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Make use of PageFilter
        /// </summary>
        [Fact]
        public async Task Create_Page_With_PageFilter_PrePaged()
        {
            var data = UserData.UserList;

            var totalItemCount = data.Count();

            var smallDataSet = data.Skip(8).Take(2).ToList();

            // Set the page size on 2 and the index on 5
            var filterModel = new PageFilter(5, 2);

            var result = await smallDataSet.ToPageAsync(filterModel, totalItemCount);

            // Take the 8the and 9the item
            var expected = smallDataSet;

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Create a Page from a empty List
        /// </summary>
        [Fact]
        public async Task Create_Empty_Page()
        {
            var result = await new List<User>().ToPageAsync();

            var expected = new List<User>().ToArray();

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Create a Page from an pre-paged list
        /// </summary>
        [Fact]
        public async Task Create_Page_With_Prepaged_Zero()
        {
            var data = UserData.UserList;

            // Create a Page for 15 items on page 5, with an total of 0 items
            var result = await data.ToPageAsync(5, 15, 0);

            var expected = new List<User>();

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Create page form EF
        /// </summary>
        [Fact]
        public async Task Create_Page_From_EF01()
        {
            /// mock EF
            var data = UserData.UserList
                .AsQueryable();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<UserContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            /// Call Service with mock EF
            var service = new UserService(mockContext.Object);
            var result = await service.GetPageUsersAsync(new PageFilter(2, 5), default);

            var expected = data.Skip(5).Take(5).ToList();

            Assert.Equal(2, result.PageIndex);
            Assert.Equal(expected, result, new UserComparer());
        }
    }
}
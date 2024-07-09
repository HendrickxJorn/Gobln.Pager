using Gobln.Pager;
using Gobln.PagerTest.Data;
using Gobln.PagerTest.EFHelper;
using Gobln.PagerTest.Helper;
using Gobln.PagerTest.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Gobln.PagerTest
{
    public class PageListAsyncTest
    {
        [Fact]
        public async Task Create_PagedList_From_EF01()
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
            var result = await service.GetPagedListUsersAsync(default);

            var expected = data;

            Assert.Equal(expected, result, new UserComparer());
        }
    }
}
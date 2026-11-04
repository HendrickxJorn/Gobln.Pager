using Gobln.Pager;
using Gobln.Pager.TestHelper.Comparer;
using Gobln.Pager.TestHelper.Data;
using Gobln.Pager.TestHelper.EFHelper;
using Gobln.Pager.TestHelper.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Gobln.PagerTest;

public class PageListAsyncTest
{
    /// <summary>
    /// Create pagedList form EF
    /// </summary>
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
        mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator);

        var mockContext = new Mock<UserContext>();
        mockContext.Setup(c => c.Users).Returns(mockSet.Object);

        /// Call Service with mock EF
        var service = new UserService(mockContext.Object);
        var result = await service.GetPagedListUsersAsync(default);

        var expected = data;

        Assert.Equal(expected, result, new UserComparer());
    }

    /// <summary>
    /// A cancelled token cancels the work before it starts
    /// </summary>
    [Fact]
    public async Task Create_PagedList_Cancelled_01()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => UserData.UserList.ToPagedListAsync(cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => UserData.UserList.ToPagedListAsync(10, cancellation.Token));
    }
}
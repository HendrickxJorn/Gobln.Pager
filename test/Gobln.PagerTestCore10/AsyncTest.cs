using Gobln.Pager;
using Gobln.PagerTestCore10.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Gobln.PagerTestCore10
{
    [TestClass]
    public class AsyncTest
    {
        /// <summary>
        /// Create Page async
        /// </summary>
        [TestMethod]
        public void PageAsyncTest1()
        {
            Task<Page<TestModel1>> task = HelperList.List1Amount15.ToPageAsync();

            task.Wait();

            var result = task.Result;

            result = result;
        }

        /// <summary>
        /// Create Page async
        /// </summary>
        [TestMethod]
        public void PageAsyncTest2()
        {
            var task = HelperList.List1Amount15.ToPageAsync(1, 3);

            task.Wait();

            var result = task.Result;

            result = result;
        }

        /// <summary>
        /// Create Page async from PagedList and use prepaged
        /// </summary>
        [TestMethod]
        public void PageAsyncTest3()
        {
            var task = HelperList.PagedList1Amount15.ToPageAsync(1, 3);

            task.Wait();

            var page = task.Result;

            task = page.ToPageAsync(new PagerFilter() { PageIndex = 1, PageSize = 3 }, 15, true);

            task.Wait();

            page = task.Result;

            page = page;
        }
    }
}
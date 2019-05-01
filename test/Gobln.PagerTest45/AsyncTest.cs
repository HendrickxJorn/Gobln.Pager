using Gobln.Pager;
using Gobln.PagerTest45.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Gobln.PagerTest45
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

            var resultPage = task.Result;

            var resultList = resultPage.ToList();

            var expected = HelperList.List1Amount15;

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());
        }

        /// <summary>
        /// Create Page async
        /// </summary>
        [TestMethod]
        public void PageAsyncTest2()
        {
            var task = HelperList.List1Amount15.ToPageAsync(1, 3);

            task.Wait();

            var resultPage = task.Result;

            var resultList = resultPage.ToList();

            var expected = HelperList.List1Amount15.Take(3).ToList();

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());
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

            var resultPage = task.Result;

            var resultList = resultPage.ToList();

            var expected = HelperList.List1Amount15.Take(3).ToList();

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());
        }
    }
}
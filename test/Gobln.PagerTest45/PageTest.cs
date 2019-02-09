using Gobln.Pager;
using Gobln.PagerTest45.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Gobln.PagerTest45
{
    [TestClass]
    public class PageTest
    {
        /// <summary>
        /// Create a pager from a List
        /// </summary>
        [TestMethod]
        public void PageTest1()
        {
            var page = HelperList.List1Amount15.ToPage();

            page = page;
        }

        /// <summary>
        /// Create a pager from a PagedList
        /// </summary>
        [TestMethod]
        public void PageTest2()
        {
            var page = HelperList.PagedList1Amount15.ToPage(5, 15);

            page = page;
        }

        /// <summary>
        /// Create a pager from an pre paged list
        /// </summary>
        [TestMethod]
        public void PageTest3()
        {
            var prePaged = HelperList.List1Amount15.ToPage(5, 15, 100, prePaged: true);

            prePaged = prePaged;
        }

        /// <summary>
        /// Test with empty
        /// </summary>
        [TestMethod]
        public void PageTest4()
        {
            var testList = new List<TestModel1>()
            {
            };

            var page = new Page<TestModel1>(testList);

            testList = null;

            page = new Page<TestModel1>(testList);

            page = testList.ToPage(1, 10, 0);

            page = testList.ToPage(1, 10, 0, prePaged: true);

            page = page;
        }

        /// <summary>
        /// Make use of IPagerFilter
        /// </summary>
        [TestMethod]
        public void PageTest5()
        {
            var filterModel = new FilterModel()
            {
                PageIndex = 5,
                PageSize = 2
            };

            var page = HelperList.List1Amount15.ToPage(filterModel);

            page = page;
        }

        /// <summary>
        /// Test Prepage
        /// </summary>
        [TestMethod]
        public void PageTest6()
        {
            var filterModel = new FilterModel()
            {
                PageIndex = 5,
                PageSize = 2
            };

            var prePaged = HelperList.List1Amount15.ToPage(filterModel);

            var page = prePaged
                .Select(c => c.Name)
                .ToPage(prePaged.CurrentPageIndex, prePaged.PageSize, prePaged.TotalItemCount, prePaged: true);

            page = page;
        }
    }
}
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
            var resultPage = HelperList.List1Amount15.ToPage();

            var resultList = resultPage.ToList();

            var expected = HelperList.List1Amount15;

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());
        }

        /// <summary>
        /// Create a pager from a PagedList
        /// </summary>
        [TestMethod]
        public void PageTest2()
        {
            var resultPage = HelperList.PagedList1Amount15.ToPage(5, 15);

            var resultList = resultPage.ToList();

            var expected = new List<TestModel1>();

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());
        }

        /// <summary>
        /// Create a pager from an pre paged list
        /// </summary>
        [TestMethod]
        public void PageTest3()
        {
            var resultPage = HelperList.List1Amount15.ToPage(5, 15, 100, prePaged: true);

            var resultList = resultPage.ToList();

            var expected = HelperList.List1Amount15;

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());
        }

        /// <summary>
        /// Test with empty
        /// </summary>
        [TestMethod]
        public void PageTest4()
        {
            var testList = new List<TestModel1>();

            var resultPage = new Page<TestModel1>(testList);

            var resultList = resultPage.ToList();

            var expected = testList;

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());



            testList = null;

            resultPage = new Page<TestModel1>(testList);

            resultList = resultPage.ToList();

            expected = new List<TestModel1>();

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());



            resultPage = testList.ToPage(1, 10, 0);

            resultList = resultPage.ToList();

            expected = new List<TestModel1>();

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());



            resultPage = testList.ToPage(1, 10, 0, prePaged: true);

            resultList = resultPage.ToList();

            expected = new List<TestModel1>();

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());
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

            var resultPage = HelperList.List1Amount15.ToPage(filterModel);

            var resultList = resultPage.ToList();

            var expected = HelperList.List1Amount15.Skip(8).Take(2).ToList();

            CollectionAssert.AreEqual(expected, resultList, new TestModel1Comparer());
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

            var resultPage = prePaged
                .Select(c => c.Name)
                .ToPage(prePaged.CurrentPageIndex, prePaged.PageSize, prePaged.TotalItemCount, prePaged: true);

            var resultList = resultPage.ToList();

            var expected = HelperList.List1Amount15.Select(c => c.Name).Skip(8).Take(2).ToList();

            CollectionAssert.AreEqual(expected, resultList);
        }
    }
}
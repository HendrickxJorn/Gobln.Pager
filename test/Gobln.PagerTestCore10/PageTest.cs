using Gobln.Pager;
using Gobln.PagerTestCore10.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gobln.PagerTestCore10
{
    [TestClass]
    public class PageTest
    {
        /// <summary>
        /// Create a pager
        /// </summary>
        [TestMethod]
        public void PageTest1()
        {
            var testList = new List<TestModel1>()
            {
                new TestModel1(){ Id = 1, Name = "Tester1", Date = new DateTime( 2016, 5,1 ) },
                new TestModel1(){ Id = 2, Name = "Tester2", Date = new DateTime( 2016, 5,2 ) },
                new TestModel1(){ Id = 3, Name = "Tester3", Date = new DateTime( 2016, 5,3 ) },
                new TestModel1(){ Id = 4, Name = "Tester4", Date = new DateTime( 2016, 5,4 ) },
                new TestModel1(){ Id = 5, Name = "Tester5", Date = new DateTime( 2016, 5,5 ) },
            };

            var page = testList.ToPage();
            
            page = page;
        }

        /// <summary>
        /// Create a pager
        /// </summary>
        [TestMethod]
        public void PageTest2()
        {
            var testList = new PagedList<TestModel1>()
            {
                new TestModel1(){ Id = 1, Name = "Tester1", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 2, Name = "Tester2", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 3, Name = "Tester3", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 4, Name = "Tester4", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 5, Name = "Tester5", Date = new DateTime( 2015, 5,5 ) },
                new TestModel1(){ Id = 6, Name = "Tester6", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 7, Name = "Tester7", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 8, Name = "Tester8", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 9, Name = "Tester9", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 10, Name = "Tester10", Date = new DateTime( 2015, 5,5 ) },
                new TestModel1(){ Id = 11, Name = "Tester11", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 12, Name = "Tester12", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 13, Name = "Tester13", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 14, Name = "Tester14", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 15, Name = "Tester15", Date = new DateTime( 2015, 5,5 ) },
            };

            testList.Add(new TestModel1() { Id = 16, Name = "Tester16", Date = new DateTime(2015, 5, 5) });

            var page = testList.GetCurrentPage();
            
            page = page;
        }

        /// <summary>
        /// Create a pager from an pre paged list
        /// </summary>
        [TestMethod]
        public void PageTest3()
        {
            var testList = new List<TestModel1>()
            {
                new TestModel1(){ Id = 1, Name = "Tester1", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 2, Name = "Tester2", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 3, Name = "Tester3", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 4, Name = "Tester4", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 5, Name = "Tester5", Date = new DateTime( 2015, 5,5 ) },
                new TestModel1(){ Id = 6, Name = "Tester6", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 7, Name = "Tester7", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 8, Name = "Tester8", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 9, Name = "Tester9", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 10, Name = "Tester10", Date = new DateTime( 2015, 5,5 ) },
            };

            var page = testList.ToPage(5, 10, 100, prePaged: true);
            
            page = page;
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
            var testList = new List<TestModel1>()
            {
                new TestModel1(){ Id = 1, Name = "Tester1", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 2, Name = "Tester2", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 3, Name = "Tester3", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 4, Name = "Tester4", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 5, Name = "Tester5", Date = new DateTime( 2015, 5,5 ) },
                new TestModel1(){ Id = 6, Name = "Tester6", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 7, Name = "Tester7", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 8, Name = "Tester8", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 9, Name = "Tester9", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 10, Name = "Tester10", Date = new DateTime( 2015, 5,5 ) },
                new TestModel1(){ Id = 11, Name = "Tester11", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 12, Name = "Tester12", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 13, Name = "Tester13", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 14, Name = "Tester14", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 15, Name = "Tester15", Date = new DateTime( 2015, 5,5 ) },
            };

            var filterModel = new FilterModel()
            {
                PageIndex = 5,
                PageSize = 2
            };

            var page = testList.ToPage(filterModel);
            
            page = page;
        }

        /// <summary>
        /// Test Prepage
        /// </summary>
        [TestMethod]
        public void PageTest6()
        {
            var testList = new List<TestModel1>()
            {
                new TestModel1(){ Id = 1, Name = "Tester1", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 2, Name = "Tester2", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 3, Name = "Tester3", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 4, Name = "Tester4", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 5, Name = "Tester5", Date = new DateTime( 2015, 5,5 ) },
                new TestModel1(){ Id = 6, Name = "Tester6", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 7, Name = "Tester7", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 8, Name = "Tester8", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 9, Name = "Tester9", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 10, Name = "Tester10", Date = new DateTime( 2015, 5,5 ) },
                new TestModel1(){ Id = 11, Name = "Tester11", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 12, Name = "Tester12", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 13, Name = "Tester13", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 14, Name = "Tester14", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 15, Name = "Tester15", Date = new DateTime( 2015, 5,5 ) },
            };

            var filterModel = new FilterModel()
            {
                PageIndex = 5,
                PageSize = 2
            };

            var page = testList.ToPage(filterModel);


            var prePage = page
                .Select(c => c.Name)
                .ToPage(page.CurrentPageIndex, page.PageSize, page.TotalItemCount, prePaged: true);
            
            prePage = prePage;
        }
    }
}
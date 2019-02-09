using Gobln.Pager;
using Gobln.PagerTest45.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gobln.PagerTest45
{
    [TestClass]
    public class PageListTest
    {
        /// <summary>
        /// Create PageList
        /// </summary>
        [TestMethod]
        public void PageListTest1()
        {
            var pagedList = new PagedList<TestModel1>()
            {
                new TestModel1(){ Id = 1, Name = "Tester1", Date = new DateTime( 2016, 5,1 ) },
                new TestModel1(){ Id = 2, Name = "Tester2", Date = new DateTime( 2016, 5,2 ) },
                new TestModel1(){ Id = 3, Name = "Tester3", Date = new DateTime( 2016, 5,3 ) },
                new TestModel1(){ Id = 4, Name = "Tester4", Date = new DateTime( 2016, 5,4 ) },
                new TestModel1(){ Id = 5, Name = "Tester5", Date = new DateTime( 2016, 5,5 ) },
            };

            pagedList = pagedList;
        }

        /// <summary>
        /// Create PageList and add item
        /// </summary>
        [TestMethod]
        public void PageListTest2()
        {
            var pagedList = new PagedList<TestModel1>()
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

            pagedList.Add(new TestModel1() { Id = 16, Name = "Tester16", Date = new DateTime(2015, 5, 5) });

            pagedList = pagedList;
        }

        /// <summary>
        /// Create PageList from List
        /// </summary>
        [TestMethod]
        public void PageListTest3()
        {
            var pagedList = new List<TestModel1>(HelperList.List1Amount15);

            pagedList.Add(new TestModel1() { Id = 16, Name = "Tester16", Date = new DateTime(2015, 5, 5) });

            pagedList = pagedList;
        }

        /// <summary>
        /// Create PageList from List
        /// </summary>
        [TestMethod]
        public void PageListTest4()
        {
            var pagedList = new List<TestModel1>();

            pagedList.AddRange(HelperList.List1Amount15);

            pagedList.Add(new TestModel1() { Id = 16, Name = "Tester16", Date = new DateTime(2015, 5, 5) });

            pagedList = pagedList;
        }

        /// <summary>
        /// Create PageList
        /// </summary>
        [TestMethod]
        public void PageListTest5()
        {
            var testList = HelperList.List1Amount15
                .OrderBy(c => c.Date)
                .ToList();

            //PagedList is orderd by Date
            var pageList = testList.ToPagedList();

            //Reorder PageList
            pageList = pageList
                .OrderBy(c => c.Id)
                .ToPagedList();

            //Create List
            testList = pageList.ToList();

            testList = testList;

            //create Array
            var testArray = pageList.ToArray();

            testArray = testArray;
        }

        /// <summary>
        /// Create PageList and converts to Pager
        /// </summary>
        [TestMethod]
        public void PageListTest6()
        {
            var pagedList = HelperList.PagedList1Amount15;

            //Create default pager
            var page = pagedList.GetCurrentPage();

            //set page values
            pagedList.CurrentPageIndex = 2;
            pagedList.PageSize = 3;

            page = pagedList.GetCurrentPage();

            // get the 4 page
            // this will set the CurrentPageIndex to 4
            page = pagedList.GetPage(4);

            //Get the next pager
            //This will set the CurrentPageIndex by 1
            page = pagedList.GetNextPage();

            //Get the previous pager
            //This will set the CurrentPageIndex by -1
            page = pagedList.GetPreviousPage();

            //peak at the next pager
            //This will NOT set the CurrentPageIndex by 1
            page = pagedList.PeakNextPage();

            //peak the previous pager
            //This will NOT set the CurrentPageIndex by -1
            page = pagedList.PeakPreviousPage();
        }
    }
}
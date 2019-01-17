using Gobln.Pager;
using Gobln.PagerTestCore10.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gobln.PagerTestCore10
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
            var testPageList = new PagedList<TestModel1>()
            {
                new TestModel1(){ Id = 1, Name = "Tester1", Date = new DateTime( 2016, 5,1 ) },
                new TestModel1(){ Id = 2, Name = "Tester2", Date = new DateTime( 2016, 5,2 ) },
                new TestModel1(){ Id = 3, Name = "Tester3", Date = new DateTime( 2016, 5,3 ) },
                new TestModel1(){ Id = 4, Name = "Tester4", Date = new DateTime( 2016, 5,4 ) },
                new TestModel1(){ Id = 5, Name = "Tester5", Date = new DateTime( 2016, 5,5 ) },
            };
            
            testPageList = testPageList;
        }

        /// <summary>
        /// Create PageList and add item
        /// </summary>
        [TestMethod]
        public void PageListTest2()
        {
            var testPageList = new PagedList<TestModel1>()
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

            testPageList.Add(new TestModel1() { Id = 16, Name = "Tester16", Date = new DateTime(2015, 5, 5) });
            
            testPageList = testPageList;
        }

        /// <summary>
        /// Create PageList and adds range
        /// </summary>
        [TestMethod]
        public void PageListTest3()
        {
            var testPageList = new List<TestModel1>();

            testPageList.AddRange(new List<TestModel1>
            {
                new TestModel1() { Id = 1, Name = "Tester1", Date = new DateTime(2015, 5, 1) },
                new TestModel1() { Id = 2, Name = "Tester2", Date = new DateTime(2015, 5, 2) },
                new TestModel1() { Id = 3, Name = "Tester3", Date = new DateTime(2015, 5, 3) },
                new TestModel1() { Id = 4, Name = "Tester4", Date = new DateTime(2015, 5, 4) },
                new TestModel1() { Id = 5, Name = "Tester5", Date = new DateTime(2015, 5, 5) },
                new TestModel1() { Id = 6, Name = "Tester6", Date = new DateTime(2015, 5, 1) },
                new TestModel1() { Id = 7, Name = "Tester7", Date = new DateTime(2015, 5, 2) },
                new TestModel1() { Id = 8, Name = "Tester8", Date = new DateTime(2015, 5, 3) },
                new TestModel1() { Id = 9, Name = "Tester9", Date = new DateTime(2015, 5, 4) },
                new TestModel1() { Id = 10, Name = "Tester10", Date = new DateTime(2015, 5, 5) },
            });

            testPageList.Add(new TestModel1() { Id = 16, Name = "Tester16", Date = new DateTime(2015, 5, 5) });
            
            testPageList = testPageList;
        }

        /// <summary>
        /// Create PageList and convert to list or array
        /// </summary>
        [TestMethod]
        public void PageListTest4()
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

            testList = testList.OrderBy(c => c.Date).ToList();

            var testPageList = testList.ToPagedList();

            testPageList = testPageList.OrderBy(c => c.Id).ToPagedList();

            var testList2 = testPageList.ToList();

            var testArray = testPageList.ToArray();
        }

        /// <summary>
        /// Create PageList and converts to Pager
        /// </summary>
        [TestMethod]
        public void PageListTest5()
        {
            var testPageList = new PagedList<TestModel1>()
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

            //Create default pager
            var pager = testPageList.GetCurrentPage();

            //set page values
            testPageList.CurrentPageIndex = 2;
            testPageList.PageSize = 3;

            pager = testPageList.GetCurrentPage();

            // get the 4 page
            // this will set the CurrentPageIndex to 4
            pager = testPageList.GetPage(4);

            //Get the next pager
            //This will set the CurrentPageIndex by 1
            pager = testPageList.GetNextPage();

            //Get the previous pager
            //This will set the CurrentPageIndex by -1
            pager = testPageList.GetPreviousPage();

            //peak at the next pager
            //This will NOT set the CurrentPageIndex by 1
            pager = testPageList.PeakNextPage();

            //peak the previous pager
            //This will NOT set the CurrentPageIndex by -1
            pager = testPageList.PeakPreviousPage();
        }
    }
}
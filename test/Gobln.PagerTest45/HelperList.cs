using Gobln.Pager;
using Gobln.PagerTest45.Models;
using System;
using System.Collections.Generic;

namespace Gobln.PagerTest45
{
    public static class HelperList
    {
        public static List<TestModel1> List1Amount15
        {
            get
            {
                return new List<TestModel1>()
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
                    new TestModel1() { Id = 11, Name = "Tester11", Date = new DateTime(2015, 5, 1) },
                    new TestModel1() { Id = 12, Name = "Tester12", Date = new DateTime(2015, 5, 2) },
                    new TestModel1() { Id = 13, Name = "Tester13", Date = new DateTime(2015, 5, 3) },
                    new TestModel1() { Id = 14, Name = "Tester14", Date = new DateTime(2015, 5, 4) },
                    new TestModel1() { Id = 15, Name = "Tester15", Date = new DateTime(2015, 5, 5) },
                };
            }
        }

        public static PagedList<TestModel1> PagedList1Amount15
        {
            get
            {
                return new PagedList<TestModel1>()
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
                    new TestModel1() { Id = 11, Name = "Tester11", Date = new DateTime(2015, 5, 1) },
                    new TestModel1() { Id = 12, Name = "Tester12", Date = new DateTime(2015, 5, 2) },
                    new TestModel1() { Id = 13, Name = "Tester13", Date = new DateTime(2015, 5, 3) },
                    new TestModel1() { Id = 14, Name = "Tester14", Date = new DateTime(2015, 5, 4) },
                    new TestModel1() { Id = 15, Name = "Tester15", Date = new DateTime(2015, 5, 5) },
                };
            }
        }
    }
}
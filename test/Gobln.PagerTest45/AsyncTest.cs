using Gobln.Pager;
using Gobln.PagerTest45.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobln.PagerTest45
{
    [TestClass]
    public class AsyncTest
    {
        [TestMethod]
        public void PageAsyncTest1()
        {
            var testList = new List<TestModel1>()
            {
                new TestModel1(){ Id = 1, Name = "Tester1", Date = new DateTime( 2016, 5,1 ) },
                new TestModel1(){ Id = 2, Name = "Tester2", Date = new DateTime( 2016, 5,2 ) },
                new TestModel1(){ Id = 3, Name = "Tester3", Date = new DateTime( 2016, 5,3 ) },
                new TestModel1(){ Id = 4, Name = "Tester4", Date = new DateTime( 2016, 5,4 ) },
                new TestModel1(){ Id = 5, Name = "Tester5", Date = new DateTime( 2016, 5,5 ) },
            };

            Task<Page<TestModel1>> task = testList.ToPageAsync();

            task.Wait();

            var result = task.Result;

            result = result;
        }
    }
}

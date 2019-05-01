using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobln.PagerTestCore10.Models
{
    public class TestModel1
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }
    }

    public class TestModel1Comparer : Comparer<TestModel1>
    {
        public override int Compare(TestModel1 x, TestModel1 y)
        {
            return x.Id.CompareTo(y.Id)
                & x.Name.CompareTo(y.Name)
                & x.Date.CompareTo(y.Date);
        }
    }
}

using Gobln.Pager;
using Gobln.PagerTest.Data;
using Gobln.PagerTest.Helper;
using Gobln.PagerTest.Model;
using Xunit;

namespace Gobln.PagerTest
{
    public class PageListTest
    {
        /// <summary>
        /// Create PageList
        /// </summary>
        [Fact]
        public void Create_PagedList_Initialize01()
        {
            var data = UserData.UserList;

            var result = new PagedList<User>()
            {
                new() { Id = 1, Name = "Tester1",   SurName= "Test", Date = new DateTime(2023, 5, 1) },
                new() { Id = 2, Name = "Tester2",   SurName= "Test", Date = new DateTime(2023, 5, 2) },
                new() { Id = 3, Name = "Tester3",   SurName= "Test", Date = new DateTime(2023, 5, 3) },
                new() { Id = 4, Name = "Tester4",   SurName= "Test", Date = new DateTime(2023, 5, 4) },
                new() { Id = 5, Name = "Tester5",   SurName= "Test", Date = new DateTime(2023, 5, 5) },
            };

            var expected = data.Take(5);

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Create PageList
        /// </summary>
        [Fact]
        public void Create_PagedList_Initialize02()
        {
            var data = UserData.UserList;

            var result = new PagedList<User>(data);

            var expected = data;

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Add new items
        /// </summary>
        [Fact]
        public void Create_PagedList_Add01()
        {
            var data = UserData.UserList;

            var dataToAdd1 = new User() { Id = 101, Name = "Tester101", SurName = "Test", Date = new DateTime(2023, 5, 1) };
            var dataToAdd2 = new User() { Id = 102, Name = "Tester102", SurName = "Test", Date = new DateTime(2023, 5, 1) };
            var dataToAdd3 = new User() { Id = 103, Name = "Tester103", SurName = "Test", Date = new DateTime(2023, 5, 1) };

            var result = new PagedList<User>(data);

            result.Add(dataToAdd1);
            result.Add(dataToAdd2);
            result.Add(dataToAdd3);

            var expected = data.ToList();

            expected.Add(dataToAdd1);
            expected.Add(dataToAdd2);
            expected.Add(dataToAdd3);

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Add new items
        /// </summary>
        [Fact]
        public void Create_PagedList_AddRange01()
        {
            var data = UserData.UserList;

            var dataToAdd = new List<User>()
            {
                new () { Id = 101, Name = "Tester101", SurName = "Test", Date = new DateTime(2023, 5, 1) },
                new () { Id = 102, Name = "Tester102", SurName = "Test", Date = new DateTime(2023, 5, 1) },
                new () { Id = 103, Name = "Tester103", SurName = "Test", Date = new DateTime(2023, 5, 1) },
            };

            var result = new PagedList<User>(data);

            result.AddRange(dataToAdd);

            var expected = data.ToList();

            expected.AddRange(dataToAdd);

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Use orderby
        /// </summary>
        [Fact]
        public void Create_PagedList_OrderBy01()
        {
            var data = UserData.UserList;

            var result = new PagedList<User>(data);

            result = result
                .OrderBy(x => x.Date)
                .ToPagedList();

            var expected = data
                .OrderBy(x => x.Date)
                .ToArray();

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Get Page
        /// </summary>
        [Fact]
        public void Create_PagedList_GetPage01()
        {
            var data = UserData.UserList;

            var pagedlist = new PagedList<User>(data);

            var result = pagedlist.GetPage();

            var expected = data.ToPage(1, 10);

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Get next page
        /// </summary>
        [Fact]
        public void Create_PagedList_GetPageNext01()
        {
            var data = UserData.UserList;

            var pagedlist = new PagedList<User>(data);

            var result = pagedlist.NextPage().GetPage();

            var expected = data.ToPage(2, 10);

            Assert.Equal(expected, result, new UserComparer());
        }

        /// <summary>
        /// Get Previous page
        /// </summary>
        [Fact]
        public void Create_PagedList_GetPagePrevious01()
        {
            var data = UserData.UserList;

            var pagedlist = new PagedList<User>(data);

            pagedlist.PageIndex = 3;

            var result = pagedlist.PreviousPage().GetPage();

            var expected = data.ToPage(2, 10);

            Assert.Equal(expected, result, new UserComparer());
        }


        /// <summary>
        /// Get Pages
        /// </summary>
        [Fact]
        public void Create_PagedList_GetPages01()
        {
            var data = UserData.UserList;

            var pagedlist = new PagedList<User>(data);

            var result = pagedlist.GetPages().ToList();

            var expected = new List<Page<User>>()
            {
                data.ToPage(1,10),
                data.ToPage(2,10),
                data.ToPage(3,10),
            };

            for (int i = 0; i < 2; i++) 
            {
                Assert.Equal(expected[i], result[i], new UserComparer());
            }
        }
    }
}
namespace Gobln.Pager.TestHelper.Model
{
    public class PageListInClassGenericType
    {
        public IPagedList<User> Users { get; set; }

        public int SomeNumber { get; set; } = 5;
    }
}
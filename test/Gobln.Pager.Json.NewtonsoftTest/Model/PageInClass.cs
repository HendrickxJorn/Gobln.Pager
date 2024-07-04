namespace Gobln.Pager.Json.NewtonsoftTest.Model
{
    internal class PageInClass
    {
        public Page<TestUser> User { get; set; }

        public int SomeNumber { get; set; } = 5;
    }
}
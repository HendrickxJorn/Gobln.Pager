namespace Gobln.Pager.Json.MicrosoftTest.Model
{
    internal class PageInClassGenericType
    {
        public IPage<TestUser> User { get; set; } = new Page<TestUser>();

        public int SomeNumber { get; set; } = 5;
    }
}
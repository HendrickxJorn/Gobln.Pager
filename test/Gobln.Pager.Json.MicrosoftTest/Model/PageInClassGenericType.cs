namespace Gobln.Pager.Json.MicrosoftTest.Model
{
    internal class PageInClassGenericType
    {
        public IPage<User> User { get; set; } = new Page<User>();

        public int SomeNumber { get; set; } = 5;
    }
}
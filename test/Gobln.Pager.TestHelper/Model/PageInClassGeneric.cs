namespace Gobln.Pager.TestHelper.Model;

public class PageInClassGenericType
{
    public IPage<User> User { get; set; }// = new Page<User>();

    public int SomeNumber { get; set; } = 5;
}
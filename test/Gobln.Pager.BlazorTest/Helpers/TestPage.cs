namespace Gobln.Pager.BlazorTest.Helpers;

// Minimal IPage implementation used for tests
internal class TestPage : IPage
{
    public int PageIndex { get; set; }

    public int TotalPageCount { get; set; }

    public int PageSize => throw new NotImplementedException();

    public int TotalItemCount => throw new NotImplementedException();

    public bool HasPaging { get; set; }

    public bool IsEmpty { get; set; }
}
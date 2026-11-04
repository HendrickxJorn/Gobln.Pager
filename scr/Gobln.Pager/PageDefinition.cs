namespace Gobln.Pager;

internal class PageDefinition(int pageIndex, int pageSize, int totalItemCount)
{
    // An empty source keeps the size it was asked for. Only a page made without one, such as ToPage() of nothing, has 0
    public int PageSize => pageSize > 0 ? pageSize : (totalItemCount == 0 ? 0 : int.MaxValue);

    public int PageIndex
    {
        get
        {
            {
                if (pageIndex < 1 || totalItemCount == 0)
                {
                    pageIndex = 1;
                }

                return pageIndex > PageCount && PageCount > 0
                    ? PageCount
                    : pageIndex;
            }
        }
    }

    public int PageCount => totalItemCount == 0 ? 1 : (int)Math.Ceiling(totalItemCount / (decimal)pageSize);

    // Built on the clamped PageIndex, so a page past the end skips to the last page instead of past all items
    public int SkipIndex => totalItemCount == 0 ? 0 : (PageIndex - 1) * pageSize;

    public int TotalItemCount => totalItemCount;
}
namespace Gobln.Pager;

internal class PageDefinition(int pageIndex, int pageSize, int totalItemCount)
{
    public int PageSize => totalItemCount == 0 ? 0 : (pageSize <= 0 ? int.MaxValue : pageSize);

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
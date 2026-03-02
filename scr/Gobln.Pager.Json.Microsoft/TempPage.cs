namespace Gobln.Pager.Json.Microsoft;

internal class TempPage<T>
{
    public T[]? PageItems { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int TotalItemCount { get; set; }
}
namespace Gobln.Pager;

internal static class Validator
{
    private const string GreaterThanZero = "Must be greater than zero.";
    private const string NotLessThanZero = "Can't be less than zero.";

    internal static void Validate(int pageIndex, int pageSize, int? totalItemCount = null)
    {
        if (pageIndex < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageIndex), pageIndex, GreaterThanZero);
        }

        if (!IsWithoutPageSize(pageSize, totalItemCount))
        {
            ValidatePageSize(pageSize);
        }

        if (totalItemCount is not null)
        {
            ValidateTotalItemCount(totalItemCount.Value);
        }
    }

    // pagerFilter is the parameter name of every public method that takes an IPageFilter
    internal static void Validate(IPageFilter pagerFilter, int? totalItemCount = null)
    {
        if (pagerFilter is null)
        {
            throw new ArgumentNullException(nameof(pagerFilter));
        }

        if (pagerFilter.PageIndex < 1)
        {
            throw new ArgumentOutOfRangeException($"{nameof(pagerFilter)}.{nameof(IPageFilter.PageIndex)}", pagerFilter.PageIndex, GreaterThanZero);
        }

        if (pagerFilter.PageSize < 1 && !IsWithoutPageSize(pagerFilter.PageSize, totalItemCount))
        {
            throw new ArgumentOutOfRangeException($"{nameof(pagerFilter)}.{nameof(IPageFilter.PageSize)}", pagerFilter.PageSize, GreaterThanZero);
        }

        if (totalItemCount is not null)
        {
            ValidateTotalItemCount(totalItemCount.Value);
        }
    }

    // A PageSize of 0 is what a page without a page size reports, such as new Page<T>(). It is allowed
    // with a total of 0 items, so such a page can be made again from its own values, and without a
    // total, which then is 0: the page is empty, and nothing is counted or read.
    private static bool IsWithoutPageSize(int pageSize, int? totalItemCount) => pageSize == 0 && totalItemCount is null or 0;

    internal static void ValidatePageSize(int pageSize)
    {
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, GreaterThanZero);
        }
    }

    internal static void ValidateTotalItemCount(int totalItemCount)
    {
        if (totalItemCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalItemCount), totalItemCount, NotLessThanZero);
        }
    }
}
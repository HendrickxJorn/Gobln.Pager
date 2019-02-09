using System;

namespace Gobln.Pager
{
    internal static class Validator
    {
        internal static void Validate(int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException("pageIndex", "Can not be less then zero or zero.");

            ValidatePageSize(pageSize);
        }

        internal static void Validate(IPagerFilter pagerFilter)
        {
            if (pagerFilter.PageIndex < 1)
                throw new ArgumentOutOfRangeException("IPagerFilter.PageIndex", "Can not be less then zero or zero.");

            if (pagerFilter.PageSize < 1)
                throw new ArgumentOutOfRangeException("IPagerFilter.PageSize", "Can not be less then zero or zero.");
        }

        internal static void ValidatePageSize(int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", "Can not be less then zero or zero.");
        }
    }
}
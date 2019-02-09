using System;

namespace Gobln.Pager
{
    internal static class Calculate
    {
        /// <summary>
        /// Calculate the total page count
        /// </summary>
        /// <param name="totalItems"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        internal static int TotalPageCount(int totalItems, int pageSize)
        {
            return (int)Math.Ceiling(totalItems / (decimal)pageSize);
        }

        /// <summary>
        /// Calculate the skip index
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        internal static int SkipIndex(int pageIndex, int pageSize)
        {
            return (pageIndex - 1) * pageSize;
        }

        /// <summary>
        /// Caclulate if the pageindex will overflow or underflow the current page
        /// Can never be smaller then 1
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        internal static int ValidPageIndex(int pageIndex, int pageSize, int itemCount)
        {
            if (pageIndex < 1)
                pageIndex = 1;

            var totalPageCount = TotalPageCount(itemCount, pageSize);

            return pageIndex > totalPageCount && totalPageCount > 0
                    ? totalPageCount
                    : pageIndex;
        }

        internal static int ValidPageSize(int pageSize)
        {
            return pageSize <= 0
                ? int.MaxValue
                : pageSize;
        }

        internal static int CalculatePageCount(int count, int pageSize)
        {
            return count == 0
                ? 1
                : ((count - 1) / pageSize) + 1;
        }
    }
}
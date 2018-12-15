using System;
using System.Linq;

namespace Gobln.Pager.Infrastructure
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
        /// Calculate the start record index
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        internal static int StartRecordIndex(int pageIndex, int pageSize)
        {
            return SkipIndex(pageIndex, pageSize) + 1;
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
        /// Calculate the end record index
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalItemCount"></param>
        /// <returns></returns>
        internal static int EndRecordIndex(int pageIndex, int pageSize, int totalItemCount)
        {
            var tempSize = pageIndex * pageSize;

            return totalItemCount > tempSize
                        ? tempSize
                        : totalItemCount;
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

        /// <summary>
        /// Create and array of integers from an given value to an given value
        /// The from value must be smaller or equal to the to value
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        internal static int[] GetListIntFormTo(int from, int to)
        {
            if (from >= to)
                return new int[0];

            return Enumerable.Range(from, to - from + 1).ToArray();
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
using System;

namespace Gobln.Pager
{
    internal class PageDefinition(int pageIndex, int pageSize, int totalItemCount)
    {
        public int PageSize => pageSize <= 0 ? int.MaxValue : pageSize;

        public int PageIndex
        {
            get
            {
                {
                    if (pageIndex < 1)
                    {
                        pageIndex = 1;
                    }

                    return pageIndex > PageCount && PageCount > 0
                        ? PageCount
                        : pageIndex;
                }
            }
        }

        public int PageCount => (int)Math.Ceiling(totalItemCount / (decimal)pageSize);

        public int SkipIndex => (pageIndex - 1) * pageSize;

        public int TotalItemCount => totalItemCount;
    }
}
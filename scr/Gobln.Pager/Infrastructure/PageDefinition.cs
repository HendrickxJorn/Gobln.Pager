namespace Gobln.Pager.Infrastructure
{
    internal class PageDefinition
    {
        #region Properties

        public int PageSize { get; private set; }
        public int PageIndex { get; private set; }
        public int PageCount { get; private set; }
        public int SkipIndex { get; private set; }
        public int ItemCount { get; private set; }

        #endregion Properties

        #region Init

        public PageDefinition(int pageIndex, int pageSize, int itemCount)
        {
            ItemCount = itemCount;
            PageSize = Calculate.ValidPageSize(pageSize);
            PageIndex = Calculate.ValidPageIndex(pageIndex, pageSize, itemCount);
            PageCount = Calculate.TotalPageCount(itemCount, PageSize);
            SkipIndex = Calculate.SkipIndex(pageIndex, pageSize);
        }

        #endregion Init
    }
}
namespace Gobln.PagerTestCore10.Models
{
    public class FilterModel : Gobln.Pager.IPagerFilter
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
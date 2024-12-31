using Gobln.Pager.Mvc.Contracts;

namespace Gobln.Pager.Mvc
{
    //todo ???
    public class PagerOptionsLabels : IPagerOptionsLabels
    {
        public string First { get; set; } = "<<";

        public string Previous { get; set; } = "<";

        public string Next { get; set; } = ">";

        public string Last { get; set; } = ">>";

        public string Jump { get; set; } = "...";
    }
}
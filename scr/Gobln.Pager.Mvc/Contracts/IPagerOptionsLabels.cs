namespace Gobln.Pager.Mvc.Contracts
{
    public interface IPagerOptionsLabels
    {
        string First { get; set; }

        string Previous { get; set; }

        string Next { get; set; }

        string Last { get; set; }

        string Jump { get; set; }
    }
}

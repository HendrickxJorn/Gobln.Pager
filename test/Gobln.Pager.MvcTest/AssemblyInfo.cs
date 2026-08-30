using Xunit;

// Gobln.Pager.Mvc holds its default PagerOptions in a process-wide static
// (PagerSettings.DefaultPagerOptions). The AddMvcPager tests write to it and the
// parameterless Html.Pager(page) overload reads it, so letting xUnit run these
// classes in parallel would race on shared state.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

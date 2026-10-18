namespace Gobln.Pager.Mvc;

#if NETFRAMEWORK
public static class PagerSettings
{
    /// <summary>
    /// Default <see cref="PagerOptions"/>
    /// </summary>

    public static PagerOptions DefaultPagerOptions { get; set; } = new PagerOptions();
}
#else
internal static class PagerSettings
{
    /// <summary>
    /// The library defaults, for an app that didn't register its own with AddMvcPager.
    /// Read only: an app's own defaults live in its services, not in this process-wide static.
    /// </summary>
    internal static PagerOptions DefaultPagerOptions { get; } = new PagerOptions();
}
#endif

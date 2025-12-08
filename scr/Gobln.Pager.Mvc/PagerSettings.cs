namespace Gobln.Pager.Mvc
{
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
        /// Default <see cref="PagerOptions"/>
        /// </summary>
        internal static PagerOptions DefaultPagerOptions { get; set; } = null;
    }
#endif
}

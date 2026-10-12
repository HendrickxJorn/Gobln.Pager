#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;

namespace Gobln.Pager.Mvc;

public static class PagerMvcServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMvcPager()
        {
            return services.AddMvcPager(new PagerOptions());
        }

        /// <summary>
        /// Use <paramref name="pagerOptions"/> as the defaults for <c>Html.Pager(page)</c>.
        /// The options are copied, so any <see cref="IPagerOptions"/> works and later changes
        /// to <paramref name="pagerOptions"/> don't affect the defaults.
        /// </summary>
        public IServiceCollection AddMvcPager(IPagerOptions pagerOptions)
        {
            ArgumentNullException.ThrowIfNull(pagerOptions, nameof(pagerOptions));

            PagerSettings.DefaultPagerOptions = PagerOptions.CopyOf(pagerOptions);

            return services;
        }

        public IServiceCollection AddMvcPager(Action<IPagerOptions> pagerOptions)
        {
            ArgumentNullException.ThrowIfNull(pagerOptions, nameof(pagerOptions));

            pagerOptions.Invoke(PagerSettings.DefaultPagerOptions);

            return services;
        }
    }
}

#endif
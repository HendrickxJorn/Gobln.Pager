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
        /// Use <paramref name="pagerOptions"/> as this app's defaults for <c>Html.Pager(page)</c>.
        /// The options are copied, so any <see cref="IPagerOptions"/> works and later changes
        /// to <paramref name="pagerOptions"/> don't affect the defaults.
        /// </summary>
        public IServiceCollection AddMvcPager(IPagerOptions pagerOptions)
        {
            ArgumentNullException.ThrowIfNull(pagerOptions, nameof(pagerOptions));

            // Copied now, not when the options are first read, so later changes don't reach them
            var copy = PagerOptions.CopyOf(pagerOptions);

            services.Configure<PagerOptions>(options => options.CopyFrom(copy));

            return services;
        }

        /// <summary>
        /// Change this app's defaults for <c>Html.Pager(page)</c>
        /// </summary>
        public IServiceCollection AddMvcPager(Action<IPagerOptions> pagerOptions)
        {
            ArgumentNullException.ThrowIfNull(pagerOptions, nameof(pagerOptions));

            services.Configure<PagerOptions>(options => pagerOptions(options));

            return services;
        }
    }
}

#endif
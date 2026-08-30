#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;

namespace Gobln.Pager.Mvc;

public static class PagerMvcServiceCollectionExtensions
{
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-9.0

    extension(IServiceCollection services)
    {
        public IServiceCollection AddMvcPager()
        {
            return services.AddMvcPager(new PagerOptions());
        }

        public IServiceCollection AddMvcPager(IPagerOptions pagerOptions)
        {
            ArgumentNullException.ThrowIfNull(pagerOptions, nameof(pagerOptions));

            PagerSettings.DefaultPagerOptions = (PagerOptions)pagerOptions;

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
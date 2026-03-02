#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;

namespace Gobln.Pager.Mvc;

public static class PagerMvcServiceCollectionExtensions
{
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-9.0

    public static IServiceCollection AddMvcPager(this IServiceCollection services)
    {
        return services.AddMvcPager(new PagerOptions());
    }

    public static IServiceCollection AddMvcPager(this IServiceCollection services, IPagerOptions pagerOptions)
    {
        ArgumentNullException.ThrowIfNull(pagerOptions, nameof(pagerOptions));

        PagerSettings.DefaultPagerOptions = (PagerOptions)pagerOptions;

        return services;
    }

    public static IServiceCollection AddMvcPager(this IServiceCollection services, Action<IPagerOptions> pagerOptions)
    {
        ArgumentNullException.ThrowIfNull(pagerOptions, nameof(pagerOptions));

        pagerOptions.Invoke(PagerSettings.DefaultPagerOptions);

        return services;
    }
}

#endif
using LightestNight.System.Api.Extends;
using LightestNight.System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightestNight.System.Logging.Rollbar.Extends
{
    public static class ExtendsServiceCollection
    {
        public static IServiceCollection AddRollbar(this IServiceCollection services)
        {
            services.AddApiClientFactory()
                .AddConfiguration()
                .TryAddTransient(typeof(IRollbarClient), typeof(RollbarClient));

            return services;
        }
    }
}
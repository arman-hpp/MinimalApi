using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Repositories;

namespace MinimalApi.Utils.Extensions
{
    public static class DapperServiceCollectionExtensions
    {
        public static IServiceCollection AddDapperRepository(
            this IServiceCollection serviceCollection,
            Action<RepositoryOptions> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        {
            if (!Enum.IsDefined(typeof(ServiceLifetime), contextLifetime))
                throw new InvalidEnumArgumentException(nameof(contextLifetime), (int)contextLifetime,
                    typeof(ServiceLifetime));

            if (!Enum.IsDefined(typeof(ServiceLifetime), optionsLifetime))
                throw new InvalidEnumArgumentException(nameof(optionsLifetime), (int)optionsLifetime,
                    typeof(ServiceLifetime));

            if (optionsAction != null)
                serviceCollection.AddOptions<RepositoryOptions>().Configure(optionsAction);
            return serviceCollection.AddSingleton<IDapperRepository, DapperRepository>();
        }
    }
}

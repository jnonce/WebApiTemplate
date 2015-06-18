using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// Extension methods for storing an retrieving <see cref="IApiVersionProvider"/>
    /// </summary>]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class ApiVersionProviderRetrievalExtensions
    {
        private const string ServiceStoreKey = "jnonce.WebApi.VersionedRouting.IApiVersionProvider";

        /// <summary>
        /// Sets the API version providers used globally.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="providers">The providers.</param>
        /// <exception cref="System.ArgumentNullException">providers</exception>
        public static void SetApiVersionProviders(this HttpConfiguration config, params IApiVersionProvider[] providers)
        {
            if (providers == null)
            {
                throw new ArgumentNullException("providers");
            }

            config.Properties[ServiceStoreKey] = providers;
        }

        /// <summary>
        /// Gets the API version providers configured globally.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public static IEnumerable<IApiVersionProvider> GetApiVersionProviders(this HttpConfiguration config)
        {
            object service;
            if (!config.Properties.TryGetValue(config, out service))
            {
                return Enumerable.Empty<IApiVersionProvider>();
            }

            return (IEnumerable<IApiVersionProvider>)service ?? Enumerable.Empty<IApiVersionProvider>();
        }

        /// <summary>
        /// Gets the API version providers for an individual request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static IEnumerable<IApiVersionProvider> GetApiVersionProviders(this HttpRequestMessage request)
        {
            var dependencyScope = request.GetDependencyScope();
            var services = dependencyScope.GetServices(typeof(IApiVersionProvider)).Cast<IApiVersionProvider>();
            if (services.Any())
            {
                return services;
            }

            return request.GetConfiguration().GetApiVersionProviders();
        }
    }
}

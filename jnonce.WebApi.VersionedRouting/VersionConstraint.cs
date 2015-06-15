using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;
using Semver;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// A constraint for an Api which matches the Api version
    /// </summary>
    public class VersionConstraint : IHttpRouteConstraint
    {
        private Func<SemVersion, bool> isSupported;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionConstraint"/> class.
        /// </summary>
        /// <param name="isSupported">Predicate identifying whether the given Api version is matched by this constraint.</param>
        public VersionConstraint(Func<SemVersion, bool> isSupported)
        {
            this.isSupported = isSupported;
        }

        /// <summary>
        /// Determines whether this instance equals a specified route.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="route">The route to compare.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="values">A list of parameter values.</param>
        /// <param name="routeDirection">The route direction.</param>
        /// <returns>
        /// True if this instance equals a specified route; otherwise, false.
        /// </returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public bool Match(
            HttpRequestMessage request, 
            IHttpRoute route, 
            string parameterName, 
            IDictionary<string, object> values, 
            HttpRouteDirection routeDirection)
        {
            if (routeDirection == HttpRouteDirection.UriResolution)
            {
                var services = GetApiVersionProviders(request);
                if (services == null)
                {
                    return false;
                }

                SemVersion version;
                if (TryGetApiVersion(services.OfType<IApiVersionProvider>(), request, out version) && this.isSupported(version))
                {
                    return true;
                }

                return false;
            }
            else
            {
                // Given that route parameters are given out of band, we can presume that
                // any url generation will match the given parameter values.
                return true;
            }
        }

        private static IEnumerable<IApiVersionProvider> GetApiVersionProviders(HttpRequestMessage request)
        {
            // TODO: Support registering providers in   request.GetConfiguration().Properties
            // We'll need some static class(es) to support:
            //
            // HttpConfiguration config = ...;
            // config.SetApiVersionProviders(new HttpHeaderApiVersionProvider(...), new AcceptHeaderApiVersionProvider(...));
            // IEnumerable<IApiVersionProvider> providers = config.GetApiVersionProviders();
            //
            // HttpRequestMessage request = ...;
            // IEnumerable<IApiVersionProvider> providers = request.GetApiVersionProviders(); // read from config AND IoC

            var dependencyScope = request.GetDependencyScope();
            var services = dependencyScope.GetServices(typeof(IApiVersionProvider));

            return services.OfType<IApiVersionProvider>();
        }

        private bool TryGetApiVersion(IEnumerable<IApiVersionProvider> providers, HttpRequestMessage request, out SemVersion version)
        {
            foreach (IApiVersionProvider provider in providers)
            {
                if (provider.TryGetApiVersion(request, out version))
                {
                    return true;
                }
            }

            version = null;
            return false;
        }
    }
}

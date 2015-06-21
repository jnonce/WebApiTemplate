using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;
using Semver;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// A constraint for an Api which matches the Api version
    /// </summary>
    public class ApiVersionRouteConstraint : IHttpRouteConstraint
    {
        private Func<SemVersion, bool> isSupported;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersionRouteConstraint"/> class.
        /// </summary>
        /// <param name="isSupported">Predicate identifying whether the given Api version is matched by this constraint.</param>
        public ApiVersionRouteConstraint(Func<SemVersion, bool> isSupported)
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
                var services = GetApiVersionProviders(request, parameterName, values);

                SemVersion version;
                return TryGetApiVersion(services, request, out version)
                    && this.isSupported(version);
            }
            else
            {
                // Given that route parameters are given out of band, we can presume that
                // any url generation will match the given parameter values.
                return true;
            }
        }

        // Get the version providers
        private IEnumerable<IApiVersionProvider> GetApiVersionProviders(
            HttpRequestMessage request,
            string parameterName,
            IDictionary<string, object> values)
        {
            // If the route contained a parsed parameter  value (from within the url) then we examine it
            object parameterValue;
            if (values.TryGetValue(parameterName, out parameterValue))
            {
                yield return new ProvidedApiVersionProvider(parameterValue);
            }

            // Take version information from a typical, url parameter.
            foreach (IApiVersionProvider provider in request.GetApiVersionProviders())
            {
                yield return provider;
            }
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

        // Api version provider which parses a single value
        // (value is extracted from a url part)
        private sealed class ProvidedApiVersionProvider : IApiVersionProvider
        {
            private object parameterValue;

            public ProvidedApiVersionProvider(object parameterValue)
            {
                this.parameterValue = parameterValue;
            }

            public bool TryGetApiVersion(HttpRequestMessage message, out SemVersion version)
            {
                version = parameterValue as SemVersion;
                if (version != null)
                {
                    return true;
                }

                string versionString = parameterValue as string;
                if (versionString != null && SemVersion.TryParse(versionString, out version))
                {
                    return true;
                }

                version = null;
                return false;
            }
        }
    }
}

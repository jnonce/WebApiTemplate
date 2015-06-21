using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// Open the route to any api version (or no-version)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class AnyApiVersionAttribute : Attribute, IHttpRouteConstraintProvider
    {
        /// <summary>
        /// Gets the constraints.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IHttpRouteConstraint>> GetConstraints()
        {
            yield return new KeyValuePair<string, IHttpRouteConstraint>(
                ApiVersionRouteConstraint.ConstraintKey,
                new AnyRequestRouteConstraint());
        }

        // Constraint which matches any request, opening the route
        private sealed class AnyRequestRouteConstraint : IHttpRouteConstraint
        {
            bool IHttpRouteConstraint.Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
            {
                return true;
            }
        }
    }
}

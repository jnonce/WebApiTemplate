using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace webapitmpl.Utility.ApiVersion
{
    /// <summary>
    /// A constraint for an Api which matches the Api version
    /// </summary>
    internal class VersionConstraint : IHttpRouteConstraint
    {
        public const string VersionHeaderName = "api-version";
        
        private Func<int?, bool> isSupported;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionConstraint"/> class.
        /// </summary>
        /// <param name="isSupported">Predicate identifying whether the given Api version is matched by this constraint.</param>
        public VersionConstraint(Func<int?, bool> isSupported)
        {
            this.isSupported = isSupported;
        }

        public bool Match(
            HttpRequestMessage request, 
            IHttpRoute route, 
            string parameterName, 
            IDictionary<string, object> values, 
            HttpRouteDirection routeDirection)
        {
            if (routeDirection == HttpRouteDirection.UriResolution)
            {
                var dependencyScope = request.GetDependencyScope();
                var provider = (IApiVersionProvider)dependencyScope.GetService(typeof(IApiVersionProvider));
                if (provider == null)
                {
                    throw new InvalidOperationException();
                }


                int version;
                if (provider.TryGetApiVersion(request, out version) && this.isSupported(version))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

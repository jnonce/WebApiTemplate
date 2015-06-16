using System.Collections.Generic;
using System.Web.Http.Routing;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// Provides route constraints
    /// </summary>
    public interface IHttpRouteConstraintProvider
    {
        /// <summary>
        /// Gets the constraints.
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, IHttpRouteConstraint>> GetConstraints();
    }
}

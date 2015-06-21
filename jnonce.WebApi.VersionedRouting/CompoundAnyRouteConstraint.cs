using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// Constrains a route by matching any of several child constraints
    /// </summary>
    internal class CompoundAnyRouteConstraint : IHttpRouteConstraint
    {
        /// <summary>Gets the child constraints that must match for this constraint to match.</summary>
        /// <returns>The child constraints that must match for this constraint to match.</returns>
        public IEnumerable<IHttpRouteConstraint> Constraints
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundAnyRouteConstraint" /> class.
        /// </summary>
        /// <param name="constraints">The child constraints that must match for this constraint to match.</param>
        public CompoundAnyRouteConstraint(IList<IHttpRouteConstraint> constraints)
        {
            if (constraints == null)
            {
                throw new ArgumentNullException("constraints");
            }

            this.Constraints = constraints;
        }

        /// <summary>
        /// Merges the peer constraints.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IHttpRouteConstraint Create(IEnumerable<IHttpRouteConstraint> values)
        {
            IHttpRouteConstraint[] allConstraintsForKey = values.ToArray();

            if (allConstraintsForKey.Length == 1)
            {
                return allConstraintsForKey[0];
            }
            else
            {
                return new CompoundAnyRouteConstraint(allConstraintsForKey);
            }
        }

        /// <summary>
        /// Determines whether this instance equals a specified route.
        /// </summary>
        /// <returns>true if this instance equals a specified route; otherwise, false.</returns>
        /// <param name="request">The request.</param>
        /// <param name="route">The route to compare.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="values">A list of parameter values.</param>
        /// <param name="routeDirection">The route direction.</param>
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            return this.Constraints.Any(
                c => c.Match(request, route, parameterName, values, routeDirection)
                );
        }
    }
}

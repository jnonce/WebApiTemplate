using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// Direct route provider which adds constraints to direct routes
    /// </summary>
    public class ConstrainingDirectRouteProvider : DefaultDirectRouteProvider
    {
        /// <summary>
        /// Creates <see cref="T:System.Web.Http.Routing.RouteEntry" /> instances based on the provided factories and action. 
        /// The route entries provide direct routing to the provided action.
        /// </summary>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="factories">The direct route factories.</param>
        /// <param name="constraintResolver">The constraint resolver.</param>
        /// <returns>
        /// A set of route entries.
        /// </returns>
        protected override IReadOnlyList<RouteEntry> GetActionDirectRoutes(HttpActionDescriptor actionDescriptor, IReadOnlyList<IDirectRouteFactory> factories, IInlineConstraintResolver constraintResolver)
        {
            IReadOnlyList<RouteEntry> result = base.GetActionDirectRoutes(actionDescriptor, factories, constraintResolver);

            if (result.Count > 0)
            {
                // Get constraints which are set at the controller level
                IEnumerable<IHttpRouteConstraintProvider> controllerConstraintProviders = 
                    actionDescriptor.ControllerDescriptor.GetCustomAttributes<IHttpRouteConstraintProvider>();
                IEnumerable<KeyValuePair<string, IHttpRouteConstraint>> controllerConstraints = GetPeerRouteConstraints(controllerConstraintProviders);

                // Get constraints which are set at action level
                IEnumerable<IHttpRouteConstraintProvider> actionConstraintProviders = 
                    actionDescriptor.GetCustomAttributes<IHttpRouteConstraintProvider>();
                IEnumerable<KeyValuePair<string, IHttpRouteConstraint>> actionConstraints = GetPeerRouteConstraints(actionConstraintProviders);

                // Add them to the routes
                AddConstraints(result, controllerConstraints.Concat(actionConstraints));
            }

            return result;
        }

        /// <summary>
        /// Creates <see cref="T:System.Web.Http.Routing.RouteEntry" /> instances based on the provided factories, controller and actions. 
        /// The route entries provided direct routing to the provided controller and can reach the set of provided actions.
        /// </summary>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <param name="actionDescriptors">The action descriptors.</param>
        /// <param name="factories">The direct route factories.</param>
        /// <param name="constraintResolver">The constraint resolver.</param>
        /// <returns>
        /// A set of route entries.
        /// </returns>
        protected override IReadOnlyList<RouteEntry> GetControllerDirectRoutes(HttpControllerDescriptor controllerDescriptor, IReadOnlyList<HttpActionDescriptor> actionDescriptors, IReadOnlyList<IDirectRouteFactory> factories, IInlineConstraintResolver constraintResolver)
        {
            IReadOnlyList<RouteEntry> result = GetControllerDirectRoutes(controllerDescriptor, actionDescriptors, factories, constraintResolver);

            if (result.Count > 0)
            {
                // Get constraints which are set at the controller level
                IEnumerable<IHttpRouteConstraintProvider> controllerConstraintProviders =
                    controllerDescriptor.GetCustomAttributes<IHttpRouteConstraintProvider>();
                IEnumerable<KeyValuePair<string, IHttpRouteConstraint>> controllerConstraints = GetPeerRouteConstraints(controllerConstraintProviders);

                // Add them to the routes
                AddConstraints(result, controllerConstraints);
            }

            return result;
        }

        // Copy constraints into each route
        private static void AddConstraints(IReadOnlyList<RouteEntry> result, IEnumerable<KeyValuePair<string, IHttpRouteConstraint>> constraints)
        {
            foreach (RouteEntry route in result)
            {
                // Routes are copied in order so that, for a given key, the last constraint will win
                // This allows Action constraints to override Controller constraints
                foreach (var constraint in constraints)
                {
                    route.Route.Constraints[constraint.Key] = constraint.Value;
                }
            }
        }

        // Gets route constraints from providers which are considered "peers" (not able to override each other)
        private static IEnumerable<KeyValuePair<string, IHttpRouteConstraint>> GetPeerRouteConstraints(IEnumerable<IHttpRouteConstraintProvider> providers)
        {
            // Get the constraints
            IEnumerable<KeyValuePair<string, IHttpRouteConstraint>> constraints =
                providers.SelectMany(provider => provider.GetConstraints());

            // Merge overlapping items
            IEnumerable<KeyValuePair<string, IHttpRouteConstraint>> grouped =
                constraints.GroupBy(
                    pair => pair.Key,
                    pair => pair.Value,
                    (key, values) => new KeyValuePair<string, IHttpRouteConstraint>(key, CompoundAnyRouteConstraint.Create(values)));

            return grouped;
        }
    }
}

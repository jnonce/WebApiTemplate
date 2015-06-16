using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    ///  Place on an action to expose it directly via a route.  The route will respect attributed constraints.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ConstrainedRouteAttribute : Attribute, IDirectRouteFactory
    {
        private readonly string _template;

        /// <summary>Gets the route template.</summary>
        /// <returns>The route template.</returns>
        public string Template
        {
            get
            {
                return this._template;
            }
        }

        /// <summary>Gets or sets the route name, if any; otherwise null.</summary>
        /// <returns>The route name, if any; otherwise null.</returns>
        public string Name
        {
            get;
            set;
        }

        /// <summary>Gets or sets the route order.</summary>
        /// <returns>The route order.</returns>
        public int Order
        {
            get;
            set;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Http.Routing.RouteFactoryAttribute" /> class.</summary>
        /// <param name="template">The route template.</param>
        public ConstrainedRouteAttribute(string template)
        {
            this._template = template;
        }

        /// <summary>Creates the route entry</summary>
        /// <returns>The created route entry.</returns>
        /// <param name="context">The context.</param>
        public RouteEntry CreateRoute(DirectRouteFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            IDirectRouteBuilder directRouteBuilder = context.CreateBuilder(this.Template);
            directRouteBuilder.Name = this.Name;
            directRouteBuilder.Order = this.Order;

            // Get the constraints
            IDictionary<string, object> constraints = directRouteBuilder.Constraints;
            if (constraints == null)
            {
                constraints = new Dictionary<string, object>();
                directRouteBuilder.Constraints = constraints;
            }

            IEnumerable<IHttpRouteConstraintProvider> controllerConstraints = context.Actions.Take(1)
                .SelectMany(a => a.ControllerDescriptor.GetCustomAttributes<IHttpRouteConstraintProvider>());
            AddConstraints(controllerConstraints, constraints);

            if (context.TargetIsAction)
            {
                IEnumerable<IHttpRouteConstraintProvider> actionConstraints = context.Actions
                    .SelectMany(a => a.GetCustomAttributes<IHttpRouteConstraintProvider>());
                AddConstraints(actionConstraints, constraints);
            }

            return directRouteBuilder.Build();
        }

        private void AddConstraints(IEnumerable<IHttpRouteConstraintProvider> providers, IDictionary<string, object> constraints)
        {
            foreach (KeyValuePair<string, IHttpRouteConstraint> current in providers.SelectMany(p => p.GetConstraints()))
            {
                constraints[current.Key] = current.Value;
            }
        }
    }
}

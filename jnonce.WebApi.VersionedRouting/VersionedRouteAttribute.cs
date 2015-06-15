using System.Collections.Generic;
using System.Web.Http.Routing;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// A versioned route endpoint
    /// </summary>
    public sealed class VersionedRouteAttribute : RouteFactoryAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedRouteAttribute"/>
        /// </summary>
        /// <param name="template">The route template.</param>
        /// <param name="version">The version this api supports.</param>
        public VersionedRouteAttribute(string template, int version)
            : base(template)
        {
            this.MinVersion = version;
            this.MaxVersion = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedRouteAttribute" />
        /// </summary>
        /// <param name="template">The route template.</param>
        /// <param name="minVersion">The minimum version.</param>
        /// <param name="maxVersion">The maximum version.</param>
         public VersionedRouteAttribute(string template, int minVersion, int maxVersion)
            : base(template)
        {
            this.MinVersion = minVersion;
            this.MaxVersion = maxVersion;
        }

        /// <summary>
        /// Gets the minimum version for the Api.
        /// </summary>
        public int MinVersion { get; private set; }

        /// <summary>
        /// Gets the maximum version for the Api.
        /// </summary>
        public int MaxVersion { get; private set; }

        /// <summary>
        /// Gets the route constraints, if any; otherwise null.
        /// </summary>
        public override IDictionary<string, object> Constraints
        {
            get
            {
                var c = new VersionConstraint(
                    givenVersion => (givenVersion >= MinVersion) && (givenVersion <= MaxVersion));

                var constraints = new HttpRouteValueDictionary();
                constraints.Add("version", c);
                return constraints;
            }
        }

    }
}

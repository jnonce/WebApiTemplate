using System.Collections.Generic;
using System.Web.Http.Routing;
using Semver;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// Place on an action to expose it directly via a route, but limit access to particular Api versions
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
            this.MinVersion = new SemVersion(version);
            this.MaxVersion = new SemVersion(version);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedRouteAttribute"/>
        /// </summary>
        /// <param name="template">The route template.</param>
        /// <param name="version">The version this api supports.</param>
        public VersionedRouteAttribute(string template, string version)
            : base(template)
        {
            this.MinVersion = SemVersion.Parse(version);
            this.MaxVersion = SemVersion.Parse(version);
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
            this.MinVersion = new SemVersion(minVersion);
            this.MaxVersion = new SemVersion(maxVersion);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedRouteAttribute" />
        /// </summary>
        /// <param name="template">The route template.</param>
        /// <param name="minVersion">The minimum version.</param>
        /// <param name="maxVersion">The maximum version.</param>
        public VersionedRouteAttribute(string template, string minVersion, string maxVersion)
            : base(template)
        {
            this.MinVersion = SemVersion.Parse(minVersion);
            this.MaxVersion = SemVersion.Parse(maxVersion);
        }

        /// <summary>
        /// Gets the minimum version for the Api.
        /// </summary>
         public SemVersion MinVersion { get; private set; }

        /// <summary>
        /// Gets the maximum version for the Api.
        /// </summary>
         public SemVersion MaxVersion { get; private set; }

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
                constraints.Add("api-version", c);
                return constraints;
            }
        }
    }
}

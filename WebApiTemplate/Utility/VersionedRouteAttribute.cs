using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace webapitmpl.Utility
{
    /// <summary>
    /// A versioned route endpoint
    /// </summary>
    internal sealed class VersionedRouteAttribute : RouteFactoryAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedRouteAttribute"/>
        /// </summary>
        /// <param name="template">The route template.</param>
        /// <param name="versions">The versions this api supports.</param>
        public VersionedRouteAttribute(string template, int version)
            : base(template)
        {
            this.MinVersion = version;
            this.MaxVersion = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedRouteAttribute"/>
        /// </summary>
        /// <param name="template">The route template.</param>
        /// <param name="versions">The versions this api supports.</param>
        public VersionedRouteAttribute(string template, int minVersion, int maxVersion)
            : base(template)
        {
            this.MinVersion = minVersion;
            this.MaxVersion = maxVersion;
        }

        public int MinVersion { get; private set; }

        public int MaxVersion { get; private set; }

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

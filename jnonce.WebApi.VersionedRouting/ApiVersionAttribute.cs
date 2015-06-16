using System;
using System.Collections.Generic;
using System.Web.Http.Routing;
using Semver;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// Limit constrained route to particular Api versions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ApiVersionAttribute : Attribute, IHttpRouteConstraintProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersionAttribute"/>
        /// </summary>
        /// <param name="version">The version this api supports.</param>
        public ApiVersionAttribute(string version)
        {
            this.MinVersion = SemVersion.Parse(version);
            this.MaxVersion = SemVersion.Parse(version);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersionAttribute" />
        /// </summary>
        /// <param name="minVersion">The minimum version.</param>
        /// <param name="maxVersion">The maximum version.</param>
        public ApiVersionAttribute(string minVersion, string maxVersion)
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
         /// Gets the constraints.
         /// </summary>
         /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IHttpRouteConstraint>> GetConstraints()
        {
            var c = new ApiVersionRouteConstraint(
                givenVersion => (givenVersion >= MinVersion) && (givenVersion <= MaxVersion));

            yield return new KeyValuePair<string, IHttpRouteConstraint>("api-version", c);
        }
    }
}

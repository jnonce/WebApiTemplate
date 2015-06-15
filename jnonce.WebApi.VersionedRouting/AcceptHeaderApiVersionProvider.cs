using System;
using System.Linq;
using System.Net.Http;
using Semver;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// Api version provider which uses a query string parameter
    /// </summary>
    public class AcceptHeaderApiVersionProvider : IApiVersionProvider
    {
        private string parameterName;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStringApiVersionProvider"/> class.
        /// </summary>
        /// <param name="parameterName">Name of the query string parameter.</param>
        public AcceptHeaderApiVersionProvider(string parameterName)
        {
            this.parameterName = parameterName;
        }

        /// <summary>
        /// Tries to get the Api version from the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="version">The version retrieved, if any.</param>
        /// <returns>
        /// true on success, false if no version was found.
        /// </returns>
        public bool TryGetApiVersion(HttpRequestMessage message, out SemVersion version)
        {
            string[] values = message
                .Headers.Accept
                .SelectMany(a => a.Parameters)
                .Where(nv => parameterName.Equals(nv.Name, StringComparison.OrdinalIgnoreCase))
                .Select(nv => nv.Value)
                .ToArray();

            SemVersion apiVersion;
            if ((values.Length == 1)
                && SemVersion.TryParse(values[0], out apiVersion))
            {
                version = apiVersion;
                return true;
            }

            version = null;
            return false;
        }
    }
}

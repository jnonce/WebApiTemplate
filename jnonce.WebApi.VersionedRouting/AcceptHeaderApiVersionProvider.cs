using System;
using System.Linq;
using System.Net.Http;
using Semver;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// Api version provider which uses the Http Accept header.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the parameter name is given as "vnd-api", then this header could be given as:
    /// </para>
    /// 
    /// <example>
    /// Accept: application/json; vnd-api=2.0
    /// </example>
    /// </remarks>
    public class AcceptHeaderApiVersionProvider : IApiVersionProvider
    {
        private string parameterName;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStringApiVersionProvider"/> class.
        /// </summary>
        /// <param name="parameterName">Name of the Accept header parameter to read.</param>
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

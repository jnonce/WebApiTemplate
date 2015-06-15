using System.Net.Http;

namespace jnonce.WebApi.VersionedRouting
{
    /// <summary>
    /// Provide Api version from a request message
    /// </summary>
    public interface IApiVersionProvider
    {
        /// <summary>
        /// Tries to get the Api version from the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="version">The version retrieved, if any.</param>
        /// <returns>true on success, false if no version was found.</returns>
        bool TryGetApiVersion(HttpRequestMessage message, out int version);
    }
}

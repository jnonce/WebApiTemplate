using Microsoft.Owin;

namespace webapitmpl.Utility
{
    public static class CorrelationIdOwinContextExtensions
    {
        private const string CorrelationIdKey = "requestCorrelationId";

        /// <summary>
        /// Gets the correlation identifier for the request.
        /// </summary>
        /// <remarks>
        /// Returns the last value assigned via SetCorrelationId.  If that method has not been called,
        /// returns null.
        /// </remarks>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static string GetCorrelationId(this IOwinContext context)
        {
            return context.Get<string>(CorrelationIdKey);
        }

        /// <summary>
        /// Sets the correlation identifier for the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        public static void SetCorrelationId(this IOwinContext context, string correlationId)
        {
            context.Set<string>(CorrelationIdKey, correlationId);
        }
    }
}

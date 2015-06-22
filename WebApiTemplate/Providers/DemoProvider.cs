using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;

namespace webapitmpl.Providers
{
    /// <summary>
    /// Service provided by DI
    /// </summary>
    public class DemoProvider
    {
        private HttpRequestMessage message;
        private IAuthenticationManager authManager;
        private ISystemClock systemClock;

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoProvider"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="oc">The oc.</param>
        /// <param name="systemClock">The system clock.</param>
        public DemoProvider(HttpRequestMessage message, Microsoft.Owin.IOwinContext oc, ISystemClock systemClock)
        {
            this.message = message;
            this.authManager = oc.Authentication;
            this.systemClock = systemClock;
        }

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <returns></returns>
        public string GetTime()
        {
            return systemClock.UtcNow.ToString();
        }

        /// <summary>
        /// Gets the user agent.
        /// </summary>
        /// <returns></returns>
        public string GetUserAgent()
        {
            return message.Headers.UserAgent.Select(v => v.Product.Name).FirstOrDefault();
        }
    }
}

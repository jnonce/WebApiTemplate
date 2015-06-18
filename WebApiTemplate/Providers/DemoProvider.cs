using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;

namespace webapitmpl.Providers
{
    public class DemoProvider
    {
        private HttpRequestMessage message;
        private IAuthenticationManager authManager;
        private ISystemClock systemClock;

        public DemoProvider(HttpRequestMessage message, Microsoft.Owin.IOwinContext oc, ISystemClock systemClock)
        {
            this.message = message;
            this.authManager = oc.Authentication;
            this.systemClock = systemClock;
        }

        public string GetTime()
        {
            return systemClock.UtcNow.ToString();
        }

        public string GetUserAgent()
        {
            return message.Headers.UserAgent.Select(v => v.Product.Name).FirstOrDefault();
        }
    }
}

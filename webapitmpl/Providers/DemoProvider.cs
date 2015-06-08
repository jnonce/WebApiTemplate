using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Owin.Security;

namespace webapitmpl.Providers
{
    public class DemoProvider
    {
        private HttpRequestMessage message;
        private IAuthenticationManager authManager;
        
        public DemoProvider(HttpRequestMessage message, Microsoft.Owin.IOwinContext oc)
        {
            this.message = message;
            this.authManager = oc.Authentication;
        }

        public string Get()
        {
            return message.Headers.UserAgent.Select(v => v.Product.Name).FirstOrDefault();
        }
    }
}

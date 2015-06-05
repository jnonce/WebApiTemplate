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
        
        public DemoProvider(HttpRequestMessage message, IAuthenticationManager authManager)
        {
            this.message = message;
            this.authManager = authManager;
        }

        public string Get()
        {
            return message.Headers.UserAgent.Select(v => v.Product.Name).FirstOrDefault();
        }
    }
}

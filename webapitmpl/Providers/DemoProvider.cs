using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace webapitmpl.Providers
{
    public class DemoProvider
    {
        private HttpRequestMessage message;
        
        public DemoProvider(HttpRequestMessage message)
        {
            this.message = message;
        }

        public string Get()
        {
            return message.Headers.UserAgent.Select(v => v.Product.Name).FirstOrDefault();
        }
    }
}

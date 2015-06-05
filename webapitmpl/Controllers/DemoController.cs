using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using webapitmpl.Providers;

namespace webapitmpl.Controllers
{
    [RoutePrefix("api")]
    public class DemoController : ApiController
    {
        private DemoProvider provider;

        public DemoController(DemoProvider provider)
        {
            this.provider = provider;
        }

        [HttpGet]
        [Route("item")]
        public string Foo()
        {
            return provider.Get();
        }

        [HttpGet]
        [Route("fail")]
        public string Fail()
        {
            throw new InvalidTimeZoneException();
        }    
    }
}

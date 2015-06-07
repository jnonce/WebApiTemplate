using System;
using System.Web.Http;
using Serilog;
using webapitmpl.Providers;

namespace webapitmpl.Controllers
{
    [RoutePrefix("api")]
    public class DemoController : ApiController
    {
        private DemoProvider provider;
        private ILogger logger;

        public DemoController(DemoProvider provider, ILogger logger)
        {
            this.provider = provider;
            this.logger = logger;
        }

        [HttpGet]
        [Route("item")]
        public string Foo(int itemId)
        {
            logger.Information("Demo action on {itemId}", itemId);
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

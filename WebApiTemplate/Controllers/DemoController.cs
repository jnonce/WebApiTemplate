using System;
using System.Linq;
using System.Web.Http;
using Serilog;
using webapitmpl.Models;
using webapitmpl.Providers;
using webapitmpl.Utility;

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

        [ValidateModel]
        [HttpPost]
        [Route("widget")]
        public IHttpActionResult CreateWidget(
            [FromBody]
            WidgetCreate widget)
        {
            return Ok();
        }

        [ValidateModel]
        [HttpPost]
        [Route("widget/{name}")]
        public IHttpActionResult UpdateWidget(
            string name,
            [FromBody]
            WidgetUpdate widget)
        {
            return CreatedAtRoute("", new object { }, widget);
        }

        [HttpGet]
        [Route("fail")]
        public string Fail()
        {
            throw new InvalidTimeZoneException();
        }    
    }
}

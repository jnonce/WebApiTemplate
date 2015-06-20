using System;
using System.Web.Http;
using jnonce.WebApi.VersionedRouting;
using Serilog;
using webapitmpl.Models;
using webapitmpl.Providers;
using webapitmpl.Utility;

namespace webapitmpl.Controllers
{
    [RoutePrefix("api")]
    [ApiVersion("1.0", "2.7")]
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
            logger.Information("Demo action v2.7 on {itemId}", itemId);
            return provider.GetUserAgent();
        }

        [HttpGet]
        [ApiVersion("4.0")]
        [Route("item", Name = "getItemV4")]
        public string Foo4(int itemId, string coolName)
        {
            logger.Information("Demo action v4 on {itemId}, {coolName}", itemId, coolName);
            string test = this.Url.Link("getItemV4", new { itemId = 1, coolName = "e" });
            return provider.GetTime();
        }

        [HttpPost]
        [Route("widget")]
        [ValidateModel]
        public IHttpActionResult CreateWidget(
            [FromBody]
            WidgetCreate widget)
        {
            if (widget == null)
            {
                return BadRequest();
            }

            return CreatedAtRoute(
                "UpdateWidget",
                new { name = widget.Name }, 
                widget);
        }

        [HttpPost]
        [Route("widget/{name}", Name = "UpdateWidget")]
        [ValidateModel]
        public IHttpActionResult UpdateWidget(
            string name,
            [FromBody]
            WidgetUpdate widget)
        {
            return Ok();
        }

        [HttpGet]
        [Route("fail")]
        public string Fail()
        {
            throw new InvalidTimeZoneException();
        }    
    }
}

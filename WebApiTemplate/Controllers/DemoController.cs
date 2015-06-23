using System;
using System.Web.Http;
using System.Web.Http.Description;
using jnonce.WebApi.VersionedRouting;
using Serilog;
using Swashbuckle.Swagger.Annotations;
using webapitmpl.Models;
using webapitmpl.Providers;
using webapitmpl.Utility;

namespace webapitmpl.Controllers
{
    /// <summary>
    /// Demo actions for the template project
    /// </summary>
    [RoutePrefix("api")]
    [ApiVersion("1.0", "2.7")]
    public class DemoController : ApiController
    {
        private DemoProvider provider;
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoController"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="logger">The logger.</param>
        public DemoController(DemoProvider provider, ILogger logger)
        {
            this.provider = provider;
            this.logger = logger;
        }

        /// <summary>
        /// Gets an item by id
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("item")]
        public string Foo(int itemId)
        {
            logger.Information("Demo action v2.7 on {itemId}", itemId);
            return provider.GetUserAgent();
        }

        /// <summary>
        /// Gets an item by id and name
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="coolName">Name of the cool.</param>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("4.0")]
        [Route("item", Name = "getItemV4")]
        public string Foo4(int itemId, string coolName)
        {
            logger.Information("Demo action v4 on {itemId}, {coolName}", itemId, coolName);
            string test = this.Url.Link("getItemV4", new { itemId = 1, coolName = "e" });
            return provider.GetTime();
        }

        /// <summary>
        /// Items the with path version.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v{api-version}/wedge")]
        public IHttpActionResult ItemWithPathVersion()
        {
            logger.Information("Item retrieved with item in path, still respecting the ApiVersionAttribute");
            return Ok();
        }

        /// <summary>
        /// Get biggs
        /// </summary>
        /// <remarks>
        /// This API is version agnostic
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [AnyApiVersion]
        [Route("biggs")]
        public IHttpActionResult ItemWithoutRouteConstraint()
        {
            logger.Information("Item retrieved ignoring controller's api version");
            return Ok();
        }

        /// <summary>
        /// Get vic.
        /// </summary>
        /// <remarks>
        /// Shows an API available on two API versions (but nothing inbetween)
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ApiVersion("7.2"), ApiVersion("7.7")]
        [Route("vic")]
        public string ItemWithTwoVersionConstraints()
        {
            logger.Information("Item retrieved from vic Version 7.2 or 7.7");
            return "vic Version 7.2 or 7.7";
        }

        /// <summary>
        /// Creates the widget.
        /// </summary>
        /// <remarks>
        /// Create a new widget, showing a particular model validation.
        /// </remarks>
        /// <param name="widget">The widget.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("widget")]
        [ValidateModel]
        [SwaggerResponse(201, "Account Created")]
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

        /// <summary>
        /// Updates the widget.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="widget">The widget.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("widget/{name}", Name = "UpdateWidget")]
        [ValidateModel]
        [ResponseType(typeof(Widget))]
        public IHttpActionResult UpdateWidget(
            string name,
            [FromBody]
            WidgetUpdate widget)
        {
            if (widget == null)
            {
                return BadRequest();
            }

            return Ok(widget);
        }

        /// <summary>
        /// Fails this instance.
        /// </summary>
        /// <remarks>
        /// Crashes the server, demonstrating error handling and logging.
        /// </remarks>
        /// <returns></returns>
        /// <exception cref="InvalidTimeZoneException"></exception>
        [HttpGet]
        [Route("fail")]
        [SwaggerResponse(500, "Operation failed")]
        public void Fail()
        {
            throw new InvalidTimeZoneException();
        }
    }
}

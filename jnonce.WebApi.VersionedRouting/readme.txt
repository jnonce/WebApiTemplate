jnonce.WebApi.VersionedRouting
==============================

Simple routing supporting parallel Api versions.

How To Use
----------

To set this up, add a versioning provider to HttpConfiguration:

    config.SetApiVersionProviders(
		new HttpHeaderApiVersionProvider("x-api-version"),
		new AcceptHeaderApiVersionProvider("vnd-api-version"),
		new QueryStringApiVersionProvider("api-version")
		);
	config.MapHttpAttributeRoutes();


Then, annotate controllers:

    [RoutePrefix("api")]
    public class WidgetController : ApiController
    {
		// GET api/widget
        [HttpGet]
        [VersionedRoute("widget", "1.0", "2.7")]
        public string WidgetV2_7()
        {
            return "2.7";
        }

		// GET api/widget
        [HttpGet]
        [VersionedRoute("widget", "3.0")]
        public string WidgetV3_0()
        {
            return "3.0";
        }
	}




Thanks to Jon Galloway
http://weblogs.asp.net/jongalloway/looking-at-asp-net-mvc-5-1-and-web-api-2-1-part-2-attribute-routing-with-custom-constraints

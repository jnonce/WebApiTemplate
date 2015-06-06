using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using webapitmpl.Configuration;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void ConfigureWebApi(
            HttpConfiguration config,
            IServiceConfiguration svcConfig,
            ContainerBuilder builder)
        {
            // Allow Autofac to setup filters
            builder.RegisterWebApiFilterProvider(config);

            // Autofac will make HttpRequestMessage available
            builder.RegisterHttpRequestMessage(config);

            // Autofac will create controllers
            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());

            // Allow config to modify WebApi
            svcConfig.Configure(config);

            // Routing
            config.MapHttpAttributeRoutes();
        }
    }
}

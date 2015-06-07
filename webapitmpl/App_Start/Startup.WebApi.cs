using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using webapitmpl.Configuration;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void ConfigureWebApi(
            IAppBuilder app,
            IServiceConfiguration svcConfig,
            IContainer container)
        {
            var config = new HttpConfiguration();

            container.UpdateRegistrations(
                builder =>
                {
                    // Allow Autofac to setup filters
                    builder.RegisterWebApiFilterProvider(config);

                    // Autofac will make HttpRequestMessage available
                    builder.RegisterHttpRequestMessage(config);

                    // Autofac will create controllers
                    builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());
                });

            // Allow config to modify WebApi
            svcConfig.Configure(config);

            // Routing
            config.MapHttpAttributeRoutes();

            // Pull the OWIN dependency scope into WebApi's request state
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacWebApi(config);

            // Activate WebAPI
            app.UseWebApi(config);
        }
    }
}

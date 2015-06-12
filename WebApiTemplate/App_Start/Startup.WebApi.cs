using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using FluentValidation.WebApi;
using Owin;
using webapitmpl.Configuration;
using webapitmpl.Utility;

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
                builder => ConfigureContainerForWebApi(config, builder)
                );

            // Allow config to modify WebApi
            svcConfig.Configure(config);

            // Routing
            config.MapHttpAttributeRoutes();

            // Validation            
            FluentValidationModelValidatorProvider.Configure(config);

            // Pull the OWIN dependency scope into WebApi's request state
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacWebApi(config);

            // Place WebApi onto the Owin pipeline
            app.UseWebApi(config);
        }

        private static void ConfigureContainerForWebApi(HttpConfiguration config, ContainerBuilder builder)
        {
            // Allow Autofac to setup filters
            builder.RegisterWebApiFilterProvider(config);

            // Autofac will make HttpRequestMessage available
            builder.RegisterHttpRequestMessage(config);

            // Autofac will create controllers
            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}

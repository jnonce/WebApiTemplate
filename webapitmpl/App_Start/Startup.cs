using System.Net.Http;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.IOFile;
using webapitmpl.Configuration;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Load configuration
            IServiceConfiguration svcConfig = ServiceConfiguration.GetCurrent();

            // Begin registering a container
            ContainerBuilder builder = new ContainerBuilder();

            // Register the configuration into the container
            builder.RegisterInstance(svcConfig).ExternallyOwned();

            // Register some Owin services in the container
            builder.RegisterModule<OwinContextModule>();

            // Register service types
            builder.RegisterType<webapitmpl.Providers.DemoProvider>()
                .InstancePerRequest();

            // Logging config
            LoggingConfiguration(app, svcConfig, builder);

            // WebApi config
            HttpConfiguration config = new HttpConfiguration();
            ConfigureWebApi(config, svcConfig, builder);

            // Build the container
            IContainer container = builder.Build();

            // Setup a dependency scope per request, at the OWIN layer
            app.UseAutofacMiddleware(container);

            // Make sure the IOwinContext is registered
            app.Use<OwinContextDependencyMiddleware>();

            // Pull the OWIN dependency scope into WebApi's request state
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacWebApi(config);

            // Activate WebAPI
            app.UseWebApi(config);
        }
    }
}

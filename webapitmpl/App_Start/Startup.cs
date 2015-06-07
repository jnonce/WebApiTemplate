using Autofac;
using Owin;
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

            // Register service types
            builder.RegisterType<webapitmpl.Providers.DemoProvider>()
                .InstancePerRequest();

            // Build the container
            IContainer container = builder.Build();

            // Setup a dependency scope per request, at the OWIN layer
            app.UseAutofacMiddleware(container);

            // Make sure the IOwinContext is registered in each request
            app.Use<OwinContextDependencyMiddleware>();

            // Logging config
            ConfigureLogging(app, svcConfig, container);

            // WebApi config
            ConfigureWebApi(app, svcConfig, container);
        }
    }
}

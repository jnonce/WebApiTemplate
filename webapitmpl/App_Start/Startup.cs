using Autofac;
using Owin;
using webapitmpl.Configuration;
using webapitmpl.Utility;

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

            builder.RegisterType<Microsoft.Owin.Infrastructure.SystemClock>()
                .As<Microsoft.Owin.Infrastructure.ISystemClock>();

            // Build the container
            IContainer container = builder.Build();

            // Setup a dependency scope per request, at the OWIN layer
            // Make IOwinContext available for use in a request
            app.UseAutofacMiddleware(container);

            // Logging config
            ConfigureLogging(app, svcConfig, container);

            // Auth
            ConfigureAuth(app, svcConfig, container);

            // WebApi config
            ConfigureWebApi(app, svcConfig, container);
        }
    }
}

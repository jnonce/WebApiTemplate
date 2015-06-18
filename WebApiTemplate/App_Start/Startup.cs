using System;
using Autofac;
using Owin;
using webapitmpl.Configuration;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// d
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configure the container
        /// </summary>
        public event Action<ContainerBuilder> ConfiguringContainer;

        /// <summary>
        /// Finalize the container after all other configuration is complete
        /// </summary>
        public event Action<ContainerBuilder> FinalizeContainer;

        public void Configuration(IAppBuilder app)
        {
            // Monitor
            ServiceConfiguration.OnStartup(this);
            ConfigurationPostCfg(app);
        }

        public void ConfigurationPostCfg(IAppBuilder app)
        {
            // Begin registering a container
            ContainerBuilder builder = new ContainerBuilder();

            // Configure types
            if (ConfiguringContainer != null)
            {
                ConfiguringContainer(builder);
            }

            // Register service types
            builder.RegisterType<webapitmpl.Providers.DemoProvider>()
                .InstancePerRequest();

            builder.RegisterType<Microsoft.Owin.Infrastructure.SystemClock>()
                .As<Microsoft.Owin.Infrastructure.ISystemClock>();

            // Build the container
            IContainer container = builder.Build();
            app.RegisterAppDisposing(container);

            // Setup a dependency scope per request, at the OWIN layer
            // Make IOwinContext available for use in a request
            app.UseAutofacMiddleware(container);

            // Logging config
            ConfigureLogging(app, container);

            // Auth
            ConfigureAuth(app, container);

            // WebApi config
            ConfigureWebApi(app, container);

            // Configure types
            if (FinalizeContainer != null)
            {
                FinalizeContainer(builder);
            }
        }
    }
}

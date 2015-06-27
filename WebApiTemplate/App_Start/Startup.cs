using System;
using System.Collections.Generic;
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
        /// Configurations the application
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configuration(IAppBuilder app)
        {
            Configuration(app, ServiceConfiguration.OnStartup);
        }

        /// <summary>
        /// Configurations the application
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="builderMethod">The container builder</param>
        public void Configuration(IAppBuilder app, Action<ContainerBuilder> builderMethod)
        {
            // Begin registering a container
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(app).ExternallyOwned();

            // The configuration system ultimately decides the services to be used
            builderMethod(builder);

            // Insert middleware which injects IOwinContexts
            builder.RegisterType<OwinStarter>()
                .SingleInstance()
                .As<IAppConfiguration>();

            // Build the container.  Build will also start any IStartable services
            // and allow them to inject themselves into the owin pipeline
            IContainer container = builder.Build();
            app.RegisterAppDisposing(container);

            var starters = container.Resolve<IEnumerable<IAppConfiguration>>();
            Configuration(app, starters);
        }

        // Configure the Owin pipeline using the given starters
        private static void Configuration(IAppBuilder app, IEnumerable<IAppConfiguration> starters)
        {
            foreach (IAppConfiguration starter in starters)
            {
                starter.Configuration(app);
            }
        }
    }
}

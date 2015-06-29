using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Owin;
using webapitmpl.Configuration;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Startup for the system
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
        public void Configuration(IAppBuilder app, Func<ContainerBuilder, object[]> builderMethod)
        {
            // Begin registering a container
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(app).ExternallyOwned();

            // The configuration system ultimately decides the services to be used
            object[] startupOrder = builderMethod(builder);

            // Insert middleware which injects IOwinContexts
            builder.RegisterType<OwinStartup>()
                .Keyed<IStartup>(OwinStartup.Id);

            object[] starterIds = builderMethod(builder);

            // Build the container.  Build will also start any IStartable services
            // and allow them to inject themselves into the owin pipeline
            IContainer container = builder.Build();
            app.RegisterAppDisposing(container);

            // Determine the starters to run
            IEnumerable<IStartup> starters = starterIds
                .Select(starterId => container.ResolveKeyed<IStartup>(starterId));

            // Run the starters on the Owin pipeline
            Configuration(app, starters);
        }

        // Configure the Owin pipeline using the given starters
        private static void Configuration(IAppBuilder app, IEnumerable<IStartup> starters)
        {
            foreach (IStartup starter in starters)
            {
                starter.Configuration(app);
            }
        }
    }
}

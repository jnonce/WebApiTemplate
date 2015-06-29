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
        public void Configuration(
            IAppBuilder app,
            Func<ContainerBuilder, Func<IContainer>, IEnumerable<IStartup>> builderMethod)
        {
            // Begin registering a container
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(app).ExternallyOwned();

            // Insert middleware which injects IOwinContexts
            builder.RegisterType<OwinStartup>()
                .Keyed<IStartup>(OwinStartup.Id);

            // The configuration system ultimately decides the services to be used
            IEnumerable<IStartup> starters = builderMethod(
                builder,
                () =>
                {
                    IContainer container = builder.Build();
                    app.RegisterAppDisposing(container);
                    return container;
                });

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

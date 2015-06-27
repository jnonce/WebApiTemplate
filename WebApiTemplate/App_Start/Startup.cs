using System;
using System.Linq;
using System.Collections.Generic;
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
        /// Common service starter id's
        /// </summary>
        public static class Starters
        {
            public static readonly object Docs = new object();

            public static readonly object Logging = new object();

            public static readonly object Owin = new object();

            public static readonly object WebApi = new object();

            public static readonly object Auth = new object();
        }

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
            builder.RegisterType<OwinStarter>()
                .SingleInstance()
                .As<IAppConfiguration>();

            // Build the container.  Build will also start any IStartable services
            // and allow them to inject themselves into the owin pipeline
            IContainer container = builder.Build();
            app.RegisterAppDisposing(container);

            var starters = container.Resolve<IEnumerable<IAppConfiguration>>();
            Configuration(
                app,
                startupOrder.Join(
                    starters,
                    orderedId => orderedId,
                    starter => starter.Id,
                    (orderedId, starter) => starter)
                );
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

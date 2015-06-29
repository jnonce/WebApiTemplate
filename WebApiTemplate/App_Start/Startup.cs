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
            Action<ContainerBuilder, Func<IContainer>> builderMethod)
        {
            // Begin registering a container
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(app).ExternallyOwned();

            // Insert middleware which injects IOwinContexts
            builder.RegisterType<OwinStartup>()
                .Keyed<IStartup>(OwinStartup.Id);

            // The configuration system ultimately decides the services to be used
            builderMethod(
                builder,
                () =>
                {
                    IContainer container = builder.Build();
                    app.RegisterAppDisposing(container);
                    return container;
                });
        }
    }
}

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
            // Begin registering a container
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(app).ExternallyOwned();

            // The configuration system ultimately decides the services to be used
            ServiceConfiguration.OnStartup(builder);

            // Insert middleware which injects IOwinContexts
            builder.RegisterType<OwinStarter>().SingleInstance().As<IStartable>();

            // Build the container.  Build will also start any IStartable services
            // and allow them to inject themselves into the owin pipeline
            IContainer container = builder.Build();
            app.RegisterAppDisposing(container);
        }
    }
}

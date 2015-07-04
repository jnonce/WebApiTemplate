using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Owin;
using Serilog;
using webapitmpl.App_Start;
using webapitmpl.Utility;
using LogEventLevel = Serilog.Events.LogEventLevel;
using TraceCategories = System.Web.Http.Tracing.TraceCategories;

namespace webapitmpl.Configuration
{
    /// <summary>
    /// Configuration for development
    /// </summary>
    internal class DevServiceConfiguration : IServiceConfiguration
    {
        public void Configure(Microsoft.Owin.Hosting.StartOptions startOptions)
        {
            startOptions.Urls.Add("http://localhost:8999");
        }

        public async Task Start(IAppBuilder app, Func<IAppBuilder, Task> runServer)
        {
            var builder = new ContainerBuilder();

            // Register common modules
            builder
                // Register primary services
                .RegisterModule<ProviderServicesModule>()
                // Setup for WebAPI
                .RegisterModule(
                    new WebApiServicesModule
                    {
                    })
                // Insert logging services
                .RegisterModule(
                    new LoggingServicesModule
                    {
                        ConfiguringLogging = ConfigureLogging
                    });

            using (IContainer container = builder.Build())
            {
                var logger = container.Resolve<Serilog.ILogger>();
                var httpConfig = container.Resolve<System.Web.Http.HttpConfiguration>();

                runServer = DelegatingServerFactory.CreateServer(
                    runServer,
                    new OwinStartup(container),
                    new LoggingStartup(logger),
                    new AuthStartup(),
                    new WebApiStartup(httpConfig, container),
                    new DocsStartup(httpConfig)
                    );
                
                await runServer(app);
            }
        }

        public Serilog.LoggerConfiguration ConfigureLogging(Serilog.LoggerConfiguration logging)
        {
            var webApiHushFilter = new LevelFromTypeLogEventFilter(
                LogEventLevel.Warning,
                new []
                {
                    TraceCategories.ActionCategory,
                    TraceCategories.ControllersCategory,
                    TraceCategories.FiltersCategory,
                    TraceCategories.FormattingCategory,
                    TraceCategories.MessageHandlersCategory,
                    TraceCategories.ModelBindingCategory,
                    // TraceCategories.RequestCategory,
                    //TraceCategories.RoutingCategory
                });

            // Allow warning and error events to show in the console
            logging = logging.WriteTo.Logger(
                nested => nested
                    .Filter.With(webApiHushFilter)
                    .WriteTo.LiterateConsole()
                );

            return logging;
        }

    }
}

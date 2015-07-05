using System;
using System.Threading.Tasks;
using System.Web.Http.Tracing;
using Autofac;
using Microsoft.Owin.Testing;
using Owin;
using Serilog;
using Serilog.Events;
using webapitmpl.App_Start;
using webapitmpl.Utility;

namespace WebApiTemplate.Test
{
    /// <summary>
    /// Utility methods to setup a test server
    /// </summary>
    public static class WebApiTemplateTestServer
    {
        public static TestServer CreateServer(Func<IAppBuilder, Func<IAppBuilder, Task>, Task> server)
        {
            return TestServer.Create(
                app =>
                {
                    var startup = new webapitmpl.Utility.DelegatingServerStartup()
                    {
                        DelegatingServer = DelegatingServerFactory.Create(server)
                    };

                    startup.Configuration(app);
                });
        }

        public static TestServer CreateServer()
        {
            return CreateServer(StandardStart);
        }

        public static LoggerConfiguration ConfigureStdLogging(LoggerConfiguration config)
        {
            var webApiHushFilter = new LevelFromTypeLogEventFilter(
                LogEventLevel.Warning,
                new[]
                {
                    TraceCategories.ActionCategory,
                    TraceCategories.ControllersCategory,
                    TraceCategories.FiltersCategory,
                    TraceCategories.FormattingCategory,
                    TraceCategories.MessageHandlersCategory,
                    TraceCategories.ModelBindingCategory,
                    TraceCategories.RequestCategory,
                    TraceCategories.RoutingCategory
                });

            return config.Filter.With(webApiHushFilter)
                .WriteTo.Console(outputTemplate: "{Timestamp:mm:ss:fff} [{Level}] {Message}{NewLine}{Exception}");
        }

        private static async Task StandardStart(IAppBuilder app, Func<IAppBuilder, Task> innerServer)
        {
            var builder = new ContainerBuilder();
            builder
                .RegisterModule<ProviderServicesModule>()
                .RegisterModule<WebApiServicesModule>()
                .RegisterModule(
                    new LoggingServicesModule
                    {
                        ConfiguringLogging = ConfigureStdLogging
                    });

            using (IContainer container = builder.Build())
            {
                var logger = container.Resolve<Serilog.ILogger>();
                var httpConfig = container.Resolve<System.Web.Http.HttpConfiguration>();

                var runServer = DelegatingServerFactory.CreateServer(
                    innerServer,
                    new OwinStartup(container),
                    new LoggingStartup(logger),
                    new WebApiStartup(httpConfig, container));

                await runServer(app);
            }
        }
    }
}

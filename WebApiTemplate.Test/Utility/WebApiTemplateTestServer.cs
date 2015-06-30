using System;
using System.Threading.Tasks;
using System.Web.Http;
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
        public static Task ServerAsync(
            Func<IAppBuilder, Func<IAppBuilder, Task>, Task> startup,
            Func<TestServer, Task> serverWait)
        {
            return webapitmpl.Program.ServerAsync(
                TestServer.Create,
                startup,
                serverWait);
        }

        public static Task ServerAsync(
            Func<TestServer, Task> serverWait)
        {
            return webapitmpl.Program.ServerAsync(
                TestServer.Create,
                StandardCfg,
                serverWait);
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

        private static async Task StandardCfg(IAppBuilder app, Func<IAppBuilder, Task> runServer)
        {
            var builder = new ContainerBuilder();
            builder
                .RegisterModule(
                    new OwinServicesModule
                    {
                        AppBuilder = app
                    })
                .RegisterModule<ProviderServicesModule>()
                .RegisterModule<WebApiServicesModule>()
                .RegisterModule(
                    new LoggingServicesModule
                    {
                        ConfiguringLogging = ConfigureStdLogging
                    });

            using (IContainer container = builder.Build())
            {
                container.RunStartup<OwinStartup>();
                container.RunStartup<LoggingStartup>();
                container.RunStartup<WebApiStartup>();
                container.Resolve<HttpConfiguration>().IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
                await runServer(app);
            }
        }
    }
}

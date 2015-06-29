using System;
using System.Web.Http.Tracing;
using Autofac;
using Microsoft.Owin.Testing;
using Serilog;
using Serilog.Events;
using webapitmpl.App_Start;
using webapitmpl.Configuration;
using webapitmpl.Utility;

namespace WebApiTemplate.Test
{
    /// <summary>
    /// Utility methods to setup a test server
    /// </summary>
    public static class WebApiTemplateTestServer
    {
        public static TestServer CreateServer(Action<ContainerBuilder, Func<IContainer>> builderMethod)
        {
            return TestServer.Create(
                app =>
                {
                    var startup = new webapitmpl.App_Start.Startup();
                    startup.Configuration(app, builderMethod);
                });
        }

        public static TestServer CreateServer()
        {
            return CreateServer(StandardCfg);
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

        private static void StandardCfg(ContainerBuilder builder, Func<IContainer> getContainer)
        {
            builder
            .RegisterModule<ProviderServicesModule>()
            .RegisterModule<WebApiServicesModule>()
            .RegisterModule(
                new LoggingServicesModule
                {
                    ConfiguringLogging = ConfigureStdLogging
                });

            Action<object> runStartup = ServiceConfiguration.GetStartupForContainerRunner(getContainer());
            runStartup(OwinStartup.Id);
            runStartup(LoggingStartup.Id);
            runStartup(WebApiStartup.Id);
        }
    }
}

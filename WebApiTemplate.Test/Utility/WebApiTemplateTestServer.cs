using System;
using System.Collections.Generic;
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
        public static TestServer CreateServer(Func<ContainerBuilder, Func<IContainer>, IEnumerable<IStartup>> builderMethod)
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

        private static IEnumerable<IStartup> StandardCfg(ContainerBuilder builder, Func<IContainer> getContainer)
        {
            builder
            .RegisterModule<ProviderServicesModule>()
            .RegisterModule<WebApiServicesModule>()
            .RegisterModule(
                new LoggingServicesModule
                {
                    ConfiguringLogging = ConfigureStdLogging
                });

            Func<object, IStartup> getStartup = ServiceConfiguration.GetStartup(getContainer());
            yield return getStartup(OwinStartup.Id);
            yield return getStartup(LoggingStartup.Id);
            yield return getStartup(WebApiStartup.Id);
        }
    }
}

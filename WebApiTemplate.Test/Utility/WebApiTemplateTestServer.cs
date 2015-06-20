using System;
using System.Web.Http.Tracing;
using Microsoft.Owin.Testing;
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
        public static TestServer CreateServer(Action<Startup> onStart)
        {
            return TestServer.Create(
                app =>
                {
                    var startup = new webapitmpl.App_Start.Startup();
                    onStart(startup);
                    startup.ConfigurationPostCfg(app);
                });
        }

        public static TestServer CreateServer()
        {
            return CreateServer(
                startup =>
                {
                    startup.ConfiguringLogging += ConfigureStdLogging;
                });
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
    }
}

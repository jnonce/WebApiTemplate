using System;
using System.Web.Http.Tracing;
using Autofac;
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
        public static TestServer CreateServer(Action<ContainerBuilder> onStart)
        {
            return TestServer.Create(
                app =>
                {
                    var startup = new webapitmpl.App_Start.Startup();
                    startup.Configuration(app, onStart);
                });
        }

        public static TestServer CreateServer()
        {
            return CreateServer(
                builder =>
                {
                    builder.RegisterModule<ProviderServicesModule>();
                    builder.RegisterModule(
                        new WebApiServicesModule
                        {
                            //ConfiguringWebApi = ConfiguringWebApi
                        });
                    builder.RegisterModule(
                        new LoggingServicesModule
                        {
                            ConfiguringLogging = ConfigureStdLogging
                        });
                });
        }

        public static TestServer CreateServer()
        {
            return CreateServer(builder =>
                builder
                .RegisterModule<ProviderServicesModule>()
                .RegisterModule(
                    new WebApiServicesModule
                    {
                        //ConfiguringWebApi = ConfiguringWebApi
                    })
                .RegisterModule(
                    new LoggingServicesModule
                    {
                        ConfiguringLogging = ConfigureStdLogging
                    })
                );
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

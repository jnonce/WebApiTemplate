using System.Web.Http;
using Autofac;
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

        public void Configure(ContainerBuilder builder)
        {
            // Register primary services
            builder.RegisterModule<ProviderServicesModule>();

            // Allow documentation to be generated
            builder.RegisterType<DocsStarter>().SingleInstance().As<IStartable>();

            // Setup for WebAPI
            builder.RegisterModule(
                new WebApiServicesModule
                {
                    ConfiguringWebApi = ConfiguringWebApi
                });

            // Insert logging services
            builder.RegisterModule(
                new LoggingServicesModule
                {
                    ConfiguringLogging = ConfigureLogging
                });
        }

        public void ConfiguringWebApi(HttpConfiguration config)
        {
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
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

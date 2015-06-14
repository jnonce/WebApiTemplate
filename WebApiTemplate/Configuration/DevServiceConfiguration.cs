using System.Web.Http;
using Serilog;
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
        private IServiceConfiguration general;

        public DevServiceConfiguration(IServiceConfiguration general)
        {
            this.general = general;
        }

        public void Configure(Microsoft.Owin.Hosting.StartOptions startOptions)
        {
            startOptions.Urls.Add("http://localhost:8999");
        }

        public void Configure(HttpConfiguration config)
        {
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
        }

        public Serilog.LoggerConfiguration Configure(Serilog.LoggerConfiguration logging)
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

            return general.Configure(logging);
        }

    }
}

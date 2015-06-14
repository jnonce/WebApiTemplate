using System.IO;
using Microsoft.Owin.Hosting;
using Serilog.Formatting.Json;
using Serilog.Sinks.RollingFile;
using webapitmpl.Utility;
using LogEventLevel = Serilog.Events.LogEventLevel;
using TraceCategories = System.Web.Http.Tracing.TraceCategories;

namespace webapitmpl.Configuration
{
    /// <summary>
    /// Service configuration for use in production
    /// </summary>
    public class ServiceConfiguration : IServiceConfiguration
    {
        /// <summary>
        /// Gets the configuration for the current process.
        /// </summary>
        /// <returns>Configuration</returns>
        public static IServiceConfiguration GetCurrent()
        {
            return new DevServiceConfiguration(
                new ServiceConfiguration()
                );
        }
        
        public void Configure(StartOptions startOptions)
        {
            // Scheme: https
            // Hostname: accept all requests on all network adaptors for a given port
            // Port: 443
            // VirtualDirectory: (root)
            startOptions.Urls.Add("https://+:443");
        }

        public void Configure(System.Web.Http.HttpConfiguration config)
        {
            // Do not expose errors to callers
            config.IncludeErrorDetailPolicy = System.Web.Http.IncludeErrorDetailPolicy.Never;
        }

        public Serilog.LoggerConfiguration Configure(Serilog.LoggerConfiguration logging)
        {
            // Ignore anything short of a warning from WebApi
            // However, don't hush the "System.Web.Http.Request" source, as it gives us Request start/end timings
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
                    TraceCategories.RoutingCategory,
                });


            // Log Informationals to a rolling file
            logging = logging.WriteTo.Logger(
                nested => nested
                    .MinimumLevel.Information()
                    .Filter.With(webApiHushFilter)
                    .WriteTo.Sink(GetFileSink())
                );

            return logging;
        }

        private static RollingFileSink GetFileSink()
        {
            var host = typeof(ServiceConfiguration).Assembly;

            string folder = Path.GetDirectoryName(host.Location);
            string fname = Path.Combine(folder, host.GetName().Name);

            var fileSink = new RollingFileSink(
                fname + ".{Date}.log",
                new JsonFormatter(),
                fileSizeLimitBytes: 1 << 24 /* 16 MB */,
                retainedFileCountLimit: 10);
            return fileSink;
        }
    }
}

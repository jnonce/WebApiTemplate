using System.IO;
using Microsoft.Owin.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;
using Serilog.Sinks.RollingFile;
using webapitmpl.Utility;
using LogEventLevel = Serilog.Events.LogEventLevel;

namespace webapitmpl.Configuration
{
    public interface IServiceConfiguration
    {
        void Configure(StartOptions startOptions);

        void Configure(System.Web.Http.HttpConfiguration config);

        Serilog.LoggerConfiguration Configure(Serilog.LoggerConfiguration logging);
    }

    public class ServiceConfiguration : IServiceConfiguration
    {
        public static IServiceConfiguration GetCurrent()
        {
            return new ServiceConfiguration();
        }

        public void Configure(StartOptions startOptions)
        {
            startOptions.Urls.Add("http://localhost:8999");
        }

        public void Configure(System.Web.Http.HttpConfiguration config)
        {

        }

        public Serilog.LoggerConfiguration Configure(Serilog.LoggerConfiguration logging)
        {

            // Allow warning and error events to show in the console
            logging = logging.WriteTo.Logger(
                nested => nested.WriteTo.LiterateConsole(),
                LogEventLevel.Warning
                );

            // Log Informationals to a rolling file
            // (NOTE: We exclude some noise from WebApi informational messages)
            logging = logging.WriteTo.Logger(
                nested => nested
                    .MinimumLevel.Information()
                    .Filter.With(GetWebApiHushFilter())
                    .WriteTo.Sink(GetFileSink())
                );

            return logging;
        }

        private static ILogEventFilter GetWebApiHushFilter()
        {
            // Ignore anything short of a warning from WebApi
            // However, don't hush the "System.Web.Http.Request" source, as it gives us Request start/end timings
            return new LevelFromTypeLogEventFilter(
                LogEventLevel.Warning,
                "System.Web.Http.MessageHandlers",
                "System.Web.Http.Controllers",
                "System.Web.Http.Action",
                "System.Web.Http.ModelBinding",
                "System.Net.Http.Formatting");
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

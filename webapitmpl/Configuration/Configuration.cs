using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Owin.Hosting;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.IOFile;
using Serilog.Sinks.RollingFile;

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
            string folder = Path.GetDirectoryName(
                typeof(ServiceConfiguration).Assembly.Location
                );

            string fname = Path.Combine(folder, "runtimelog");

            return logging
                .WriteTo.Logger(
                    nested => nested.WriteTo.LiterateConsole(),
                    Serilog.Events.LogEventLevel.Warning
                    )
                .WriteTo.Logger(
                    nested => nested.WriteTo.Sink(new RollingFileSink(fname + "-{Date}.log", new JsonFormatter(), fileSizeLimitBytes: 1 << 20, retainedFileCountLimit: 31)),
                    Serilog.Events.LogEventLevel.Information
                    )
                    ;
        }
    }
}

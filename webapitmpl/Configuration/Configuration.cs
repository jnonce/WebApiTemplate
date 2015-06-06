using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Hosting;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.IOFile;

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
            string fname = @"c:\Users\Justin\Documents\Visual Studio 2013\Projects\webapitmpl\webapitmpl\App_Start\stuff";
            return logging
                .WriteTo.Logger(
                    nested => nested.WriteTo.LiterateConsole(),
                    Serilog.Events.LogEventLevel.Warning
                    )
                .WriteTo.Logger(
                    nested => nested.WriteTo.Sink(new FileSink(fname + ".log", new JsonFormatter(), null)),
                    Serilog.Events.LogEventLevel.Information
                    )
                .WriteTo.Logger(
                    nested => nested.WriteTo.Sink(new FileSink(fname + ".err", new JsonFormatter(), null)),
                    Serilog.Events.LogEventLevel.Warning
                    );
        }
    }
}

using System.Net.Http;
using System.Web.Http.Tracing;
using Autofac;
using Microsoft.Owin.Logging;
using Owin;
using Serilog;
using SerilogWeb.Owin;
using webapitmpl.Configuration;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void ConfigureLogging(
            IAppBuilder app,
            IServiceConfiguration svcConfig,
            IContainer container)
        {
            // Setup logging basics
            var loggingCfg = new LoggerConfiguration();

            // Allow the logging context (CallContext) to enrich logs
            loggingCfg = loggingCfg.Enrich.FromLogContext();

            // Ensure only basics of hte HttpRequestMessage are ever logged
            loggingCfg = loggingCfg.Destructure.ByTransforming<HttpRequestMessage>(
                m => new
                {
                    Uri = m.RequestUri.AbsoluteUri,
                    m.Method.Method
                });

            // Ensure that a TraceRecord (WebApi) stores only the data we need
            loggingCfg = loggingCfg.Destructure.ByTransforming<TraceRecord>(
                tr => new
                {
                    tr.Category,
                    tr.Kind,
                    tr.Operator,
                    tr.Operation,
                    tr.Message,
                    tr.Status
                });

            // Pass logging config to configuration to setup sinks appropriate for our environment
            loggingCfg = loggingCfg.WriteTo.Logger(ll => svcConfig.Configure(ll));
                
            // Create the logger
            Serilog.ILogger logger = loggingCfg.CreateLogger();            
            Log.Logger = logger;

            // Update the container
            container.UpdateRegistrations(
                builder =>
                {
                    // Ensure that ILogger registrations are handled
                    builder.RegisterModule<AutofacSerilogBridgeModule>();

                    // Make the true logger available
                    builder.RegisterInstance(logger)
                        .ExternallyOwned();

                    // Register WebApi logging
                    builder.RegisterType<SerilogWebApiTraceWriter>()
                        .As<System.Web.Http.Tracing.ITraceWriter>();
                });

            // Register Owin logging
            app.UseSerilogRequestContext();
            app.SetLoggerFactory(new SerilogWeb.Owin.LoggerFactory(logger));

            // Initial log
            logger.ForContext<Startup>().Information("Server Started");
        }
    }
}

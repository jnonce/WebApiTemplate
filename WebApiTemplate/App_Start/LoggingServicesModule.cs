using System;
using Autofac;
using Serilog;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Define the logging characteristics of the application.
    /// </summary>
    internal class LoggingServicesModule : Module
    {
        public Func<LoggerConfiguration, LoggerConfiguration> ConfiguringLogging;

        protected override void Load(ContainerBuilder builder)
        {
            // Setup logging basics
            var loggingCfg = new LoggerConfiguration();

            // Allow the logging context (CallContext) to enrich logs
            loggingCfg = loggingCfg.Enrich.FromLogContext();

            // Pass logging config to configuration to setup sinks appropriate for our environment
            if (ConfiguringLogging != null)
            {
                loggingCfg = loggingCfg.WriteTo.Logger(ll => ConfiguringLogging(ll));
            }

            // Create the logger
            Serilog.ILogger logger = loggingCfg.CreateLogger();
            Log.Logger = logger;

            // Ensure that ILogger registrations are handled
            builder.RegisterModule<AutofacSerilogBridgeModule>();

            // Make the true logger available
            builder.RegisterInstance(logger)
                .ExternallyOwned();

            // Register WebApi logging
            builder.RegisterType<SerilogWebApiTraceWriter>()
                .As<System.Web.Http.Tracing.ITraceWriter>();
        }
    }
}

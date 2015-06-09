﻿using System;
using System.Net.Http;
using System.Text.RegularExpressions;
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

            // Log correlation id's on Http requests
            app.AcceptCorrelationId("x-correlation-id", NormalizeCorrelationId);
            app.EmitCorrelationId("x-correlation-id");
            app.LogCorrelationId("RequestCorrelation");

            // Initial log
            logger.ForContext<Startup>().Information("Server Started");
        }

        private static string NormalizeCorrelationId(string givenId)
        {
            if (givenId == null
                || givenId.Length > 100
                || givenId.Length < 1
                || !Regex.IsMatch(givenId, @"^[-_\.a-zA-Z0-9]*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
            {
                return Guid.NewGuid().ToString();
            }

            return givenId;
        }
    }
}

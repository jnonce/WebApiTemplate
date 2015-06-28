﻿using System;
using System.Text.RegularExpressions;
using Microsoft.Owin.Logging;
using Owin;
using SerilogWeb.Owin;
using webapitmpl.Utility;


namespace webapitmpl.App_Start
{
    internal class LoggingStarter : IAppConfiguration, IDisposable
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public static readonly object Id = new object();

        private Serilog.ILogger logger;

        public LoggingStarter(Serilog.ILogger logger)
        {
            this.logger = logger.ForContext<Startup>();
        }

        object IAppConfiguration.Id
        {
            get { return Id; }
        }

        public void Configuration(IAppBuilder app)
        {
            // Register Owin logging
            app.UseSerilogRequestContext();
            app.SetLoggerFactory(new SerilogWeb.Owin.LoggerFactory(logger));

            // Log correlation id's on Http requests
            app.AcceptCorrelationId("x-correlation-id", NormalizeCorrelationId);
            app.EmitCorrelationId("x-correlation-id");
            app.LogCorrelationId("RequestCorrelation");

            // Initial log
            logger.Information("Server Started");
        }

        public void Dispose()
        {
            logger.Information("Server Stopped");
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
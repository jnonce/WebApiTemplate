using System;
using Microsoft.Owin.Logging;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Bridges OWIN logging into Serilog.
    /// </summary>
    public class SerilogOwinLoggerFactory : ILoggerFactory
    {
        private Serilog.ILogger logger;

        public SerilogOwinLoggerFactory(Serilog.ILogger logger)
        {
            this.logger = logger.ForContext<SerilogOwinLoggerFactory>();
        }

        public ILogger Create(string name)
        {
            return new Logger(logger, name);
        }

        private class Logger : ILogger
        {
            private Serilog.ILogger logger;

            public Logger(Serilog.ILogger logger, string name)
            {
                this.logger = logger.ForContext("OwinSource", name);
            }

            public bool WriteCore(
                System.Diagnostics.TraceEventType eventType,
                int eventId,
                object state,
                Exception exception,
                Func<object, Exception, string> formatter)
            {
                var level = GetLevel(eventType);
                if (logger.IsEnabled(level))
                {
                    logger.Write(level, exception, "OWIN {Id} {State}", eventId, state);
                    return true;
                }

                return false;
            }

            private static Serilog.Events.LogEventLevel GetLevel(System.Diagnostics.TraceEventType eventType)
            {
                switch (eventType)
                {
                    case System.Diagnostics.TraceEventType.Critical:
                        return Serilog.Events.LogEventLevel.Fatal;

                    case System.Diagnostics.TraceEventType.Error:
                        return Serilog.Events.LogEventLevel.Error;

                    case System.Diagnostics.TraceEventType.Information:
                        return Serilog.Events.LogEventLevel.Information;

                    case System.Diagnostics.TraceEventType.Verbose:
                        return Serilog.Events.LogEventLevel.Verbose;

                    case System.Diagnostics.TraceEventType.Warning:
                        return Serilog.Events.LogEventLevel.Warning;
                    
                    default:
                        return Serilog.Events.LogEventLevel.Debug;
                }
            }
        }
    }
}

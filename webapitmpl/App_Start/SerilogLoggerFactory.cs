using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Logging;

namespace webapitmpl.App_Start
{
    public class SerilogLoggerFactory : ILoggerFactory
    {
        private Serilog.ILogger logger;

        public SerilogLoggerFactory(Serilog.ILogger logger)
        {
            this.logger = logger;
        }

        public ILogger Create(string name)
        {
            return new Logger(logger, name);
        }

        private class Logger : ILogger
        {
            private Serilog.ILogger logger;
            private string name;

            public Logger(Serilog.ILogger logger, string name)
            {
                this.logger = logger;
                this.name = name;
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

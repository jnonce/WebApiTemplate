using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Tracing;
using Serilog;
using Serilog.Events;

namespace webapitmpl.App_Start
{
    class SerilogTraceWriter : ITraceWriter
    {
        private ILogger logger;

        public SerilogTraceWriter(ILogger logger)
        {
            this.logger = logger;
        }

        private ILogger GetContextLogger(HttpRequestMessage request)
        {
            if (request == null)
            {
                return logger;
            }

            object result;
            if (request.Properties.TryGetValue("SerilogContext", out result))
            {
                return (ILogger)result;
            }

            ILogger clog = logger.ForContext<SerilogTraceWriter>();
            request.Properties.Add("SerilogContext", clog);
            return clog;
        }

        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            LogEventLevel logLevel = GetLogLevel(level);
            if (logger.IsEnabled(logLevel))
            {
                var record = new TraceRecord(request, category, level);
                traceAction(record);

                GetContextLogger(request).Write(logLevel, record.Exception, "WebApi {@TraceRecord}", record);                
            }
        }

        private static LogEventLevel GetLogLevel(TraceLevel level)
        {
            switch (level)
            {
                case TraceLevel.Fatal:
                    return LogEventLevel.Fatal;
                case TraceLevel.Error:
                    return LogEventLevel.Error;
                case TraceLevel.Warn:
                    return LogEventLevel.Warning;
                case TraceLevel.Info:
                    return LogEventLevel.Information;
                case TraceLevel.Debug:
                    return LogEventLevel.Debug;
                case TraceLevel.Off:
                default:
                    return LogEventLevel.Verbose;
            }
        }
    }
}

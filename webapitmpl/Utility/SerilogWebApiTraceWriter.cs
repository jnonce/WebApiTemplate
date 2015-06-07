using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Tracing;
using Serilog;
using Serilog.Events;

namespace webapitmpl.Utility
{
    /// <summary>
    /// WebApi trace writer which bridges <see cref="TraceRecord"/> objects into Serilog
    /// </summary>
    internal class SerilogWebApiTraceWriter : ITraceWriter
    {
        private ILogger logger;

        public SerilogWebApiTraceWriter(ILogger logger)
        {
            this.logger = logger.ForContext<SerilogWebApiTraceWriter>();
        }

        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            LogEventLevel logLevel = GetLogLevel(level);
            if (logger.IsEnabled(logLevel))
            {
                var record = new TraceRecord(request, category, level);
                traceAction(record);

                logger.Write(logLevel, record.Exception, "{@TraceRecord}", record);               
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

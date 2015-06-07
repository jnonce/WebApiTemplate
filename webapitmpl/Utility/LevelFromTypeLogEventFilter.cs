﻿using System;
using Serilog.Core;
using Serilog.Events;
using SerilogCoreConstants = Serilog.Core.Constants;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Log filter which removes events from a given source which are beneath a given level
    /// </summary>
    internal class LevelFromTypeLogEventFilter : ILogEventFilter
    {
        private ScalarValue sourceContextType;
        private LogEventLevel minimumLevel;

        public LevelFromTypeLogEventFilter(Type sourceContextType, LogEventLevel minimumLevel)
        {
            this.sourceContextType = new ScalarValue(sourceContextType.FullName);
            this.minimumLevel = minimumLevel;
        }

        public static ILogEventFilter For<T>(LogEventLevel minimumLevel)
        {
            return new LevelFromTypeLogEventFilter(typeof(T), minimumLevel);
        }

        // Return true to keep the event
        public bool IsEnabled(LogEvent logEvent)
        {
            // If the event meets the limiting level then keep it (source doesn't matter)
            if (logEvent.Level >= minimumLevel)
            {
                return true;
            }

            // If the event does not come from the expected source then we won't filter it out
            LogEventPropertyValue value;
            if (!logEvent.Properties.TryGetValue(SerilogCoreConstants.SourceContextPropertyName, out value)
                || !sourceContextType.Equals(value))
            {
                return true;
            }

            // Event came from our source and didn't meet our level; reject
            return false;
        }
    }
}
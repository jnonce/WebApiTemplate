using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace WebApiTemplate.Test
{
    internal class TestLogEventSink : ILogEventSink
    {
        private TestContext testContext;

        public TestLogEventSink(TestContext testContext)
        {
            this.testContext = testContext;
        }

        public void Emit(LogEvent logEvent)
        {
            this.testContext.WriteLine(logEvent.RenderMessage());
        }
    }
}

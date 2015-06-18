using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Tracing;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;
using webapitmpl.App_Start;
using webapitmpl.Utility;

namespace WebApiTemplate.Test
{
    [TestClass]
    public class DemoControllerVersioningTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task TestGetItem27()
        {
            using (var server = CreateServer(
                startup =>
                {
                    startup.ConfiguringLogging += ConfigureLogging;
                })
                )
            {
                string headerValue = Guid.NewGuid().ToString();

                HttpResponseMessage response = await
                    server.CreateRequest("api/item?itemId=22&coolName=g")
                    .AddHeader("api-version", "2.0")
                    .AddHeader("User-Agent", headerValue)
                    .GetAsync();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(headerValue, await response.Content.ReadAsAsync<string>());
            }
        }

        [TestMethod]
        public async Task TestGetItem30()
        {
            using (var server = CreateServer(
                startup =>
                {
                    startup.ConfiguringLogging += ConfigureLogging;
                })
                )
            {
                HttpResponseMessage response = await
                    server.CreateRequest("api/item?itemId=22&coolName=g")
                    .AddHeader("api-version", "3.0")
                    .GetAsync();
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task TestGetItem40()
        {
            using (var server = CreateServer(
                startup =>
                {
                    startup.ConfiguringLogging += ConfigureLogging;
                })
                )
            {
                HttpResponseMessage response = await
                    server.CreateRequest("api/item?itemId=22&coolName=g")
                    .AddHeader("api-version", "4.0")
                    .GetAsync();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        public TestServer CreateServer(Action<Startup> onStart)
        {
            return TestServer.Create(
                app =>
                {
                    var startup = new webapitmpl.App_Start.Startup();
                    onStart(startup);
                    startup.ConfigurationPostCfg(app);
                });
        }

        private LoggerConfiguration ConfigureLogging(LoggerConfiguration config)
        {
            var sink = new TestLogEventSink(this.TestContext);
            var webApiHushFilter = new LevelFromTypeLogEventFilter(
                LogEventLevel.Warning,
                new []
                {
                    TraceCategories.ActionCategory,
                    TraceCategories.ControllersCategory,
                    TraceCategories.FiltersCategory,
                    TraceCategories.FormattingCategory,
                    TraceCategories.MessageHandlersCategory,
                    TraceCategories.ModelBindingCategory,
                    TraceCategories.RequestCategory,
                    TraceCategories.RoutingCategory
                });

            return config
                .Filter.With(webApiHushFilter)
                .WriteTo.Sink(sink);
        }
    }
}

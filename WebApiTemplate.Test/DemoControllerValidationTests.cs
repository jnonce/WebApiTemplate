using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http.Tracing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;
using webapitmpl.Models;
using webapitmpl.Utility;

namespace WebApiTemplate.Test
{
    /// <summary>
    /// Unit tests for the demo controller's model validation
    /// </summary>
    [TestClass]
    public class DemoControllerValidationTests
    {
        private JsonMediaTypeFormatter jsonMediaTypeFormatter;

        public TestContext TestContext { get; set; }

        public DemoControllerValidationTests()
        {
            this.jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
            this.jsonMediaTypeFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        }

        [TestMethod]
        public async Task CreateWidget()
        {
            await WebApiTemplateTestServer.ServerAsync(
                async server =>
                {
                    Widget postingWidget = new Widget { };
                    HttpResponseMessage response = await server.CreateRequest("api/widget")
                        .AddHeader("api-version", "2.7")
                        .And(msg => msg.Content = new ObjectContent<Widget>(postingWidget, jsonMediaTypeFormatter))
                        .PostAsync();

                    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

                    var err = await response.Content.ReadAsAsync<HttpError>(new[] { this.jsonMediaTypeFormatter });
                    Assert.IsNotNull(err.Message);
                    CollectionAssert.AreEquivalent(
                        new[] { "'Name' must not be empty." },
                        (string[])err.ModelState["widget.Name"]);
                });
        }

        void startup_ConfiguringWebApi(System.Web.Http.HttpConfiguration httpConfiguration)
        {
            httpConfiguration.IncludeErrorDetailPolicy = System.Web.Http.IncludeErrorDetailPolicy.Always;
        }

        private LoggerConfiguration ConfigureLogging(LoggerConfiguration config)
        {
            var webApiHushFilter = new LevelFromTypeLogEventFilter(
                LogEventLevel.Warning,
                new[]
                {
                    TraceCategories.ActionCategory,
                    TraceCategories.ControllersCategory,
                    TraceCategories.FiltersCategory,
                    TraceCategories.FormattingCategory,
                    TraceCategories.MessageHandlersCategory,
                    //TraceCategories.ModelBindingCategory,
                    TraceCategories.RequestCategory,
                    TraceCategories.RoutingCategory
                });

            return config.Filter.With(webApiHushFilter)
                .WriteTo.Console(outputTemplate: "{Timestamp:mm:ss} [{Level}] {Message}{NewLine}{Exception}");
        }
    }
}

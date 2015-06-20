using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApiTemplate.Test
{
    /// <summary>
    /// Unit tests for the demo controller's error handling behavior
    /// </summary>
    [TestClass]
    public class DemoControllerErrorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task TestError()
        {
            using (var server = WebApiTemplateTestServer.CreateServer())
            {
                HttpResponseMessage response = await GetFailureResponse(server);

                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

                var errorInfo = await response.Content.ReadAsAsync<HttpError>();
                Assert.IsNull(errorInfo.ExceptionMessage);
                Assert.IsNull(errorInfo.ExceptionType);
                Assert.IsNull(errorInfo.StackTrace);
            }
        }

        [TestMethod]
        public async Task TestErrorDetail()
        {
            Action<HttpConfiguration> onInit =
                (config) =>
                {
                    config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
                };

            using (var server = WebApiTemplateTestServer.CreateServer(
                startup =>
                {
                    startup.ConfiguringLogging += WebApiTemplateTestServer.ConfigureStdLogging;
                    startup.ConfiguringWebApi += onInit;
                })
                )
            {
                HttpResponseMessage response = await GetFailureResponse(server);

                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

                var errorInfo = await response.Content.ReadAsAsync<HttpError>();
                Assert.IsNotNull(errorInfo.ExceptionMessage);
                Assert.IsNotNull(errorInfo.ExceptionType);
                Assert.IsNotNull(errorInfo.StackTrace);
            }
        }

        private static Task<HttpResponseMessage> GetFailureResponse(TestServer server)
        {
            return server.CreateRequest("api/fail")
                .AddHeader("api-version", "2.0")
                .GetAsync();
        }
    }
}

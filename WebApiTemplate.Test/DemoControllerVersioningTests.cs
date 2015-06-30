using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Owin.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace WebApiTemplate.Test
{
    /// <summary>
    /// Unit tests for the demo controller's versioned routing
    /// </summary>
    [TestClass]
    public class DemoControllerVersioningTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task TestGetItem27()
        {
            await WebApiTemplateTestServer.ServerAsync(
                async server =>
                {
                    string headerValue = Guid.NewGuid().ToString();

                    HttpResponseMessage response = await
                        server.CreateRequest("api/item?itemId=22&coolName=g")
                        .AddHeader("api-version", "2.0")
                        .AddHeader("User-Agent", headerValue)
                        .GetAsync();

                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    Assert.AreEqual(headerValue, await response.Content.ReadAsAsync<string>());
                });
        }

        [TestMethod]
        public async Task TestGetItem30()
        {
            await WebApiTemplateTestServer.ServerAsync(
                async server =>
                {
                    HttpResponseMessage response = await
                        server.CreateRequest("api/item?itemId=22&coolName=g")
                        .AddHeader("api-version", "3.0")
                        .GetAsync();
                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
                });
        }

        [TestMethod]
        public async Task TestGetItem40()
        {
            var mock = new Mock<ISystemClock>();
            mock.Setup(c => c.UtcNow)
                .Returns(new DateTimeOffset(2010, 1, 1, 12, 0, 0, TimeSpan.Zero));

            await WebApiTemplateTestServer.ServerAsync(
                async server =>
                {
                    HttpResponseMessage response = await
                        server.CreateRequest("api/item?itemId=22&coolName=g")
                        .AddHeader("api-version", "4.0")
                        .GetAsync();
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                    Assert.AreEqual("1/1/2010 12:00:00 PM +00:00", await response.Content.ReadAsAsync<string>());
                });
        }

        [TestMethod]
        public async Task TestItemWithVersionInPath()
        {
            await WebApiTemplateTestServer.ServerAsync(
                async server =>
                {
                    HttpResponseMessage response = await server.HttpClient.GetAsync("api/v2.0/wedge");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                    response = await server.HttpClient.GetAsync("api/v2.1/wedge");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                    response = await server.HttpClient.GetAsync("api/v2.9/wedge");
                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
                });
        }

        [TestMethod]
        public async Task TestAllowNoVersionOnAction()
        {
            await WebApiTemplateTestServer.ServerAsync(
                async server =>
                {
                    HttpResponseMessage response = await server.HttpClient.GetAsync("api/biggs");
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                });
        }

        [TestMethod]
        public async Task TestMultipleVersions()
        {
            await WebApiTemplateTestServer.ServerAsync(
                async server =>
                {
                    HttpResponseMessage response = await
                        server.CreateRequest("api/vic")
                        .AddHeader("api-version", "1.0")
                        .GetAsync();
                    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

                    response = await
                        server.CreateRequest("api/vic")
                        .AddHeader("api-version", "7.2")
                        .GetAsync();
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                    response = await
                        server.CreateRequest("api/vic")
                        .AddHeader("api-version", "7.7")
                        .GetAsync();
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                });
        }
    }
}

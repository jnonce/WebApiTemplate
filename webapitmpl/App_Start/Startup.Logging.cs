using System.Net.Http;
using System.Web.Http.Tracing;
using Autofac;
using Microsoft.Owin.Logging;
using Owin;
using Serilog;
using webapitmpl.Configuration;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void ConfigureLogging(
            IAppBuilder app,
            IServiceConfiguration svcConfig,
            IContainer container)
        {
            // Setup logging basics
            var logger = new LoggerConfiguration()
                .Destructure.ByTransforming<HttpRequestMessage>(
                    m => new
                    {
                        Uri = m.RequestUri.AbsoluteUri,
                        m.Method.Method
                    })
                .Destructure.ByTransforming<TraceRecord>(
                    tr => new
                    {
                        tr.Category,
                        tr.Kind,
                        tr.Operator,
                        tr.Operation,
                        tr.Message,
                        tr.Status
                    })
                .WriteTo.Logger(ll => svcConfig.Configure(ll))
                .CreateLogger();
            
            Log.Logger = logger;

            // Update the container
            container.UpdateRegistrations(
                builder =>
                {
                    // Make the logger available
                    builder.RegisterInstance(logger)
                        .ExternallyOwned();

                    // Register WebApi logging
                    builder.RegisterType<SerilogTraceWriter>()
                        .As<System.Web.Http.Tracing.ITraceWriter>();
                });

            // Register with Owin
            app.SetLoggerFactory(new SerilogLoggerFactory(logger));

            // Initial log
            logger.ForContext<Startup>().Information("Server Started");
        }
    }
}

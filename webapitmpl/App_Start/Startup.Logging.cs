using System.Net.Http;
using Autofac;
using Microsoft.Owin.Logging;
using Owin;
using Serilog;
using webapitmpl.Configuration;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void LoggingConfiguration(
            IAppBuilder app,
            IServiceConfiguration svcConfig,
            ContainerBuilder builder)
        {
            // Setup logging basics
            var logger = new LoggerConfiguration()
                .Destructure.ByTransforming<HttpRequestMessage>(
                    m => new
                    {
                        Uri = m.RequestUri.AbsoluteUri,
                        m.Method
                    })
                .WriteTo.Logger(ll => svcConfig.Configure(ll))
                .CreateLogger();
            
            Log.Logger = logger;

            // Make the logger available
            builder.RegisterInstance(logger)
                .ExternallyOwned();

            // Register WebApi logging
            builder.RegisterType<SerilogTraceWriter>()
                .As<System.Web.Http.Tracing.ITraceWriter>();

            // Register
            app.SetLoggerFactory(new SerilogLoggerFactory(logger));
        }
    }
}

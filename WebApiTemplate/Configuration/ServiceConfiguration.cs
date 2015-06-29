using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Microsoft.Owin.Hosting;
using Serilog.Formatting.Json;
using Serilog.Sinks.RollingFile;
using webapitmpl.App_Start;
using webapitmpl.Utility;
using LogEventLevel = Serilog.Events.LogEventLevel;
using TraceCategories = System.Web.Http.Tracing.TraceCategories;

namespace webapitmpl.Configuration
{
    /// <summary>
    /// Service configuration for use in production
    /// </summary>
    public class ServiceConfiguration : IServiceConfiguration
    {
        /// <summary>
        /// Gets the configuration for the current process.
        /// </summary>
        /// <returns>Configuration</returns>
        public static IServiceConfiguration GetCurrent()
        {
            return new DevServiceConfiguration();
        }

        /// <summary>
        /// Called to startup the application using the active configuration
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IEnumerable<IStartup> OnStartup(ContainerBuilder builder, Func<IContainer> getContainer)
        {
            return new DevServiceConfiguration().Configure(builder, getContainer);
        }

        /// <summary>
        /// Configures the specified startup sequence.
        /// </summary>
        /// <param name="builder">Configure the services in the container.</param>
        /// <returns>
        /// Array of objects identifying the <see cref="T:Utility.IAppConfiguration" /> to run from
        /// the container
        /// </returns>
        public IEnumerable<IStartup> Configure(ContainerBuilder builder, Func<IContainer> getContainer)
        {
            builder.RegisterModule<ProviderServicesModule>();
            builder.RegisterModule<WebApiServicesModule>();
            builder.RegisterModule(
                new LoggingServicesModule
                {
                    ConfiguringLogging = ConfigureLogging
                });

            // Support CORS
            builder.RegisterType<AuthStartup>()
                .Keyed<IStartup>(AuthStartup.Id);

            Func<object, IStartup> getStartup = GetStartup(getContainer());
            yield return getStartup(OwinStartup.Id);
            yield return getStartup(LoggingStartup.Id);
            yield return getStartup(AuthStartup.Id);
            yield return getStartup(WebApiStartup.Id);
        }

        // Utility to easily retrieve an IStartup by key from a container
        internal static Func<object, IStartup> GetStartup(IContainer container)
        {
            return key => container.ResolveKeyed<IStartup>(key);
        }

        /// <summary>
        /// Configures the specified start options.
        /// </summary>
        /// <param name="startOptions">The start options.</param>
        public void Configure(StartOptions startOptions)
        {
            // Scheme: https
            // Hostname: accept all requests on all network adaptors for a given port
            // Port: 443
            // VirtualDirectory: (root)
            startOptions.Urls.Add("https://+:443");
        }

        /// <summary>
        /// Configurings web API.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public void ConfiguringWebApi(System.Web.Http.HttpConfiguration config)
        {
            // Do not expose errors to callers
            config.IncludeErrorDetailPolicy = System.Web.Http.IncludeErrorDetailPolicy.Never;
        }

        /// <summary>
        /// Configures the logging.
        /// </summary>
        /// <param name="logging">The logging.</param>
        /// <returns></returns>
        public Serilog.LoggerConfiguration ConfigureLogging(Serilog.LoggerConfiguration logging)
        {
            // Ignore anything short of a warning from WebApi
            // However, don't hush the "System.Web.Http.Request" source, as it gives us Request start/end timings
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
                    // TraceCategories.RequestCategory,
                    TraceCategories.RoutingCategory,
                });


            // Log Informationals to a rolling file
            logging = logging.WriteTo.Logger(
                nested => nested
                    .MinimumLevel.Information()
                    .Filter.With(webApiHushFilter)
                    .WriteTo.Sink(GetFileSink())
                );

            return logging;
        }

        private static RollingFileSink GetFileSink()
        {
            var host = typeof(ServiceConfiguration).Assembly;

            string folder = Path.GetDirectoryName(host.Location);
            string fname = Path.Combine(folder, host.GetName().Name);

            var fileSink = new RollingFileSink(
                fname + ".{Date}.log",
                new JsonFormatter(),
                fileSizeLimitBytes: 1 << 24 /* 16 MB */,
                retainedFileCountLimit: 10);
            return fileSink;
        }
    }
}

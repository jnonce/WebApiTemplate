using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Owin.Hosting;
using Owin;
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
        /// Configures the specified startup sequence.
        /// </summary>
        /// <param name="builder">Configure the services in the container.</param>
        /// <param name="getContainer">Gets the container</param>
        /// <returns>
        /// Array of objects identifying the <see cref="T:Utility.IAppConfiguration" /> to run from
        /// the container
        /// </returns>
        public async Task Configure(IAppBuilder app, Func<IAppBuilder, Task> runServer)
        {
            var builder = new ContainerBuilder();

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

            using (IContainer container = builder.Build())
            {
                var runner = new Starter(container);
                runner
                    .Configuration(OwinStartup.Id)
                    .Configuration(LoggingStartup.Id)
                    .Configuration(AuthStartup.Id)
                    .Configuration<WebApiStartup>(
                        webApi =>
                        {
                            webApi.Configuration();
                        });

                await runServer(app);
            }

        }

        // Utility to easily retrieve an IStartup by key from a container
        internal static Action<object> GetStartupForContainerRunner(IComponentContext container)
        {
            return key => container.ResolveKeyed<IStartup>(key).Configuration();
        }

        internal static void RenStartupForContainerRunner<T>(IComponentContext container, Action<T> preStartup)
            where T : IStartup
        {
            var item = container.Resolve<T>();
            preStartup(item);
            item.Configuration();
        }

        internal class Starter
        {
            public Starter(IComponentContext context)
            {
                this.ComponentContext = context;
            }

            public IComponentContext ComponentContext { get; private set; }

            public Starter Configuration(object key)
            {
                ComponentContext.ResolveKeyed<IStartup>(key).Configuration();
                return this;
            }

            public Starter Configuration<T>(Action<T> preStartup)
                where T : IStartup
            {
                var item = ComponentContext.Resolve<T>();
                preStartup(item);
                item.Configuration();
                return this;
            }
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

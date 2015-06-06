using System.Net.Http;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.IOFile;
using webapitmpl.Configuration;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IServiceConfiguration svcConfig = ServiceConfiguration.GetCurrent();

            ContainerBuilder builder = new ContainerBuilder();
            HttpConfiguration config = new HttpConfiguration();

            // Setup logging basics
            LoggingConfiguration(app, svcConfig, builder);

            builder.RegisterInstance(svcConfig).ExternallyOwned();
            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterHttpRequestMessage(config);
            builder.RegisterModule<OwinContextModule>();

            builder.RegisterType<webapitmpl.Providers.DemoProvider>()
                .InstancePerRequest();

            // Routing
            svcConfig.Configure(config);
            ConfigureWebApi(config);

            // Build the container
            IContainer container = builder.Build();

            // Setup a dependency scope per request, at the OWIN layer
            app.UseAutofacMiddleware(container);

            // Make sure the IOwinContext is registered
            app.Use<OwinContextDependencyMiddleware>();

            // Pull the OWIN dependency scope into WebApi's request state
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacWebApi(config);

            //
            app.Map("/foo",
                bb =>
                {
                    bb.Run(ctx =>
                        {
                            ctx.Response.StatusCode = 200;
                            return System.Threading.Tasks.Task.FromResult(0);
                        });
                });

            // Activate WebAPI
            app.UseWebApi(config);
        }
    }
}

using System.Web.Http;
using Autofac;
using Autofac.Integration.Owin;
using Autofac.Integration.WebApi;
using Owin;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ContainerBuilder builder = new ContainerBuilder();
            HttpConfiguration config = new HttpConfiguration();

            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterHttpRequestMessage(config);
            builder.RegisterModule<OwinContextModule>();

            builder.RegisterType<webapitmpl.Providers.DemoProvider>()
                .InstancePerRequest();

            // Routing
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

            // Activate WebAPI
            app.UseWebApi(config);
        }
    }
}

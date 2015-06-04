using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;

namespace webapitmpl
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            HttpConfiguration config = new HttpConfiguration();

            containerBuilder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());
            containerBuilder.RegisterWebApiFilterProvider(config);
            containerBuilder.RegisterHttpRequestMessage(config);
            containerBuilder.RegisterType<webapitmpl.Providers.DemoProvider>()
                .InstancePerRequest();
            containerBuilder.RegisterType<webapitmpl.Providers.TraceWriter>()
                .SingleInstance()
                .AsImplementedInterfaces();
            config.MapHttpAttributeRoutes();

            IContainer container = containerBuilder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseWebApi(config);
        }
    }
}

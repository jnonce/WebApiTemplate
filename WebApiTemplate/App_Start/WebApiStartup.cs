using System;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using jnonce.WebApi.VersionedRouting;
using Owin;
using webapitmpl.Utility;


namespace webapitmpl.App_Start
{
    /// <summary>
    /// Starter which inserts WebApi into the Owin pipeline
    /// </summary>
    internal class WebApiStartup : IDelegatingServer
    {
        private ILifetimeScope container;
        private HttpConfiguration config;

        public WebApiStartup(HttpConfiguration config, ILifetimeScope madeContainer)
        {
            this.config = config;
            this.container = madeContainer;
        }

        public Task Start(IAppBuilder app, Func<IAppBuilder, Task> innerServer)
        {
            // Enforce specific Json formatting
            config.Formatters.JsonFormatter.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            // Routing: Use attribute based direct routes with Api constraints
            config.MapHttpAttributeRoutes(new ConstrainingDirectRouteProvider());

            // Validation
            config.UseAutofacFluentValidation(container);

            // Pull the OWIN dependency scope into WebApi's request state
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacWebApi(config);

            // Place WebApi onto the Owin pipeline
            app.UseWebApi(config);

            return innerServer(app);
        }
    }
}

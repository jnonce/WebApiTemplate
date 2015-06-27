using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using webapitmpl.Utility;


namespace webapitmpl.App_Start
{
    class WebApiStarter : IAppConfiguration
    {
        private ILifetimeScope container;
        private HttpConfiguration config;

        public WebApiStarter(HttpConfiguration config, ILifetimeScope madeContainer)
        {
            this.config = config;
            this.container = madeContainer;
        }

        public object Id
        {
            get { return Startup.Starters.WebApi; }
        }

        public void Configuration(IAppBuilder app)
        {
            // Validation
            config.UseAutofacFluentValidation(container);

            // Pull the OWIN dependency scope into WebApi's request state
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacWebApi(config);

            // Place WebApi onto the Owin pipeline
            app.UseWebApi(config);
        }
    }
}

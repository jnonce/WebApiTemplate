using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using webapitmpl.Utility;


namespace webapitmpl.App_Start
{
    class WebApiStarter : IStartable
    {
        private ILifetimeScope container;
        private HttpConfiguration config;
        private IAppBuilder app;

        public WebApiStarter(HttpConfiguration config, IAppBuilder app, ILifetimeScope madeContainer)
        {
            this.app = app;
            this.config = config;
            this.container = madeContainer;
        }

        public void Start()
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

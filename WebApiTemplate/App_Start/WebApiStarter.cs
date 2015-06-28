using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using webapitmpl.Utility;


namespace webapitmpl.App_Start
{
    /// <summary>
    /// Starter which inserts WebApi into the Owin pipeline
    /// </summary>
    internal class WebApiStarter : IAppConfiguration
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public static readonly object Id = new object();

        private ILifetimeScope container;
        private HttpConfiguration config;

        public WebApiStarter(HttpConfiguration config, ILifetimeScope madeContainer)
        {
            this.config = config;
            this.container = madeContainer;
        }

        object IAppConfiguration.Id
        {
            get { return Id; }
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

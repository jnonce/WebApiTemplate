using Autofac;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    internal class OwinStarter : IAppConfiguration
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public static readonly object Id = new object();

        private ILifetimeScope scope;
        
        public OwinStarter(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        object IAppConfiguration.Id
        {
            get { return Id; }
        }

        public void Configuration(IAppBuilder app)
        {
            // Setup a dependency scope per request, at the OWIN layer
            // Make IOwinContext available for use in a request
            app.UseAutofacMiddleware(scope);
        }
    }
}

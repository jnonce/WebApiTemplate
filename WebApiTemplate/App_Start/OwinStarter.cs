using Autofac;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    internal class OwinStarter : IAppConfiguration
    {
        private ILifetimeScope scope;
        
        public OwinStarter(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        public object Id
        {
            get { return Startup.Starters.Owin; }
        }

        public void Configuration(IAppBuilder app)
        {
            // Setup a dependency scope per request, at the OWIN layer
            // Make IOwinContext available for use in a request
            app.UseAutofacMiddleware(scope);
        }
    }
}

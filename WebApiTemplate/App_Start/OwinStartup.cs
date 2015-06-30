using Autofac;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    internal class OwinStartup : IStartup
    {
        private ILifetimeScope scope;
        private IAppBuilder app;
        
        public OwinStartup(IAppBuilder app, ILifetimeScope scope)
        {
            this.app = app;
            this.scope = scope;
        }

        public void Configuration()
        {
            // Setup a dependency scope per request, at the OWIN layer
            // Make IOwinContext available for use in a request
            app.UseAutofacMiddleware(scope);
        }
    }
}

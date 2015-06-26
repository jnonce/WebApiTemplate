using Autofac;
using Owin;

namespace webapitmpl.App_Start
{
    class OwinStarter : IStartable
    {
        private IAppBuilder app;
        private ILifetimeScope scope;
        
        public OwinStarter(IAppBuilder app, ILifetimeScope scope)
        {
            this.app = app;
            this.scope = scope;
        }

        public void Start()
        {
            // Setup a dependency scope per request, at the OWIN layer
            // Make IOwinContext available for use in a request
            app.UseAutofacMiddleware(scope);
        }
    }
}

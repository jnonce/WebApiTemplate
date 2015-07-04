using System;
using System.Threading.Tasks;
using Autofac;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    internal class OwinStartup : IDelegatingServer
    {
        private ILifetimeScope scope;
        
        public OwinStartup(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        public Task Start(IAppBuilder app, Func<IAppBuilder, Task> innerServer)
        {
            // Setup a dependency scope per request, at the OWIN layer
            // Make IOwinContext available for use in a request
            app.UseAutofacMiddleware(scope);
            return innerServer(app);
        }
    }
}

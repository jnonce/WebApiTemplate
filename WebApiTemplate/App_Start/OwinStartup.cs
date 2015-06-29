using Autofac;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    internal class OwinStartup : IStartup
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public static readonly object Id = new object();

        private ILifetimeScope scope;
        
        public OwinStartup(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        public void Configuration(IAppBuilder app)
        {
            // Setup a dependency scope per request, at the OWIN layer
            // Make IOwinContext available for use in a request
            app.UseAutofacMiddleware(scope);
        }
    }
}

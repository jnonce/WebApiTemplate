using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Owin;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Middleware which inserts the <see cref="IOwinContext"/> into the container
    /// </summary>
    class OwinContextDependencyMiddleware : OwinMiddleware
    {
        public OwinContextDependencyMiddleware(OwinMiddleware next)
            : base(next)
        {

        }

        public override Task Invoke(IOwinContext context)
        {
            // Get the current request's lifetime scope
            var contextScope = context.GetAutofacLifetimeScope();

            // Add the context to that scope
            var nextBuild = new ContainerBuilder();
            nextBuild.Register(c => context).InstancePerRequest().ExternallyOwned();
            nextBuild.Update(contextScope.ComponentRegistry);

            // Continue processing
            return this.Next.Invoke(context);
        }
    }
}

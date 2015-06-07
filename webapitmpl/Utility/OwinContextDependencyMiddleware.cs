using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Owin;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Middleware which inserts the <see cref="IOwinContext"/> into the container
    /// </summary>
    internal class OwinContextDependencyMiddleware : OwinMiddleware
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
            contextScope.UpdateRegistrations(
                nextBuild => RegisterServices(context, nextBuild)
                );

            // Continue processing
            return this.Next.Invoke(context);
        }

        private static void RegisterServices(IOwinContext context, ContainerBuilder nextBuild)
        {
            nextBuild.Register(c => context)
                .InstancePerRequest()
                .ExternallyOwned();

            nextBuild.Register(c => context.Request)
                .InstancePerRequest()
                .ExternallyOwned();

            nextBuild.Register(c => context.Response)
                .InstancePerRequest()
                .ExternallyOwned();

            nextBuild.Register(c => context.Authentication)
                .InstancePerRequest()
                .ExternallyOwned();
        }
    }
}

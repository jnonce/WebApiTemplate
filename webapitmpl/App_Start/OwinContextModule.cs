using Autofac;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Module which registers:
    /// 
    /// <list type="">
    ///   <item><see cref="IOwinRequest"/></item>
    ///   <item><see cref="IAuthenticationManager"/></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// This module is used with <see cref="OwinContextDependencyMiddleware"/>.
    /// </remarks>
    internal class OwinContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAdapter<IOwinContext, IOwinRequest>(
                (IOwinContext ctx) => ctx.Request);
            builder.RegisterAdapter<IOwinContext, IAuthenticationManager>(
                (IOwinContext ctx) => ctx.Authentication);
        }
    }
}

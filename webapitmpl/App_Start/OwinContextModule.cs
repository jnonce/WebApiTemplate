using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

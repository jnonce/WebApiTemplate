using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Register Owin related services
    /// </summary>
    internal class OwinServicesModule : Module
    {
        public IAppBuilder AppBuilder { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(AppBuilder).ExternallyOwned();

            // Insert middleware which injects IOwinContexts
            builder.RegisterType<OwinStartup>();
        }
    }
}

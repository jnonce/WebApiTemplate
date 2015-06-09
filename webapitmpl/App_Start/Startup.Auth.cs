using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Owin;
using webapitmpl.Configuration;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app, IServiceConfiguration svcConfig, IContainer container)
        {
			// Register authN here
            // e.g. from Microsoft.Owin.Security.*
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Cors;
using Autofac;
using Microsoft.Owin.Cors;
using Owin;
using webapitmpl.Configuration;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app, IContainer container)
        {
            // Register authN here
            // e.g. from Microsoft.Owin.Security.*

            //
            app.UseCors(CorsOptions.AllowAll);
            // http://senodio.com/site/swagger/#!/pet/addPet
        }
    }
}

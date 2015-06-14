using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;

namespace webapitmpl.Configuration
{
    public interface IServiceConfiguration
    {
        void Configure(StartOptions startOptions);

        void Configure(HttpConfiguration config);

        Serilog.LoggerConfiguration Configure(Serilog.LoggerConfiguration logging);
    }
}

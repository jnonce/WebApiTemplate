using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Owin.Hosting;

namespace webapitmpl
{
    public interface IConfiguration
    {
        void ConfigureService(StartOptions options);
    }
}

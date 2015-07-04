using System;
using System.Threading.Tasks;
using Microsoft.Owin.Cors;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Starter for authentication
    /// </summary>
    internal class AuthStartup : IDelegatingServer
    {
        public Task Start(IAppBuilder app, Func<IAppBuilder, Task> innerServer)
        {
            app.UseCors(CorsOptions.AllowAll);
            return innerServer(app);
        }
    }
}

using Microsoft.Owin.Cors;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Starter for authentication
    /// </summary>
    public class AuthStartup : IStartup
    {
        private IAppBuilder app;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthStartup"/> class.
        /// </summary>
        /// <param name="app">The Owin application.</param>
        public AuthStartup(IAppBuilder app)
        {
            this.app = app;
        }

        /// <summary>
        /// Configurations the application.
        /// </summary>
        public void Configuration()
        {
            app.UseCors(CorsOptions.AllowAll);
        }
    }
}

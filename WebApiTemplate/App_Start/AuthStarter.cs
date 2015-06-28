using Microsoft.Owin.Cors;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Starter for authentication
    /// </summary>
    public class AuthStarter : IAppConfiguration
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public static readonly object Id = new object();

        object IAppConfiguration.Id
        {
            get { return Id; }
        }

        /// <summary>
        /// Configurations the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;

namespace webapitmpl.Configuration
{
    /// <summary>
    /// Configures the service
    /// </summary>
    public interface IServiceConfiguration
    {
        /// <summary>
        /// Configures the specified start options.
        /// </summary>
        /// <param name="startOptions">The start options.</param>
        void Configure(StartOptions startOptions);

        /// <summary>
        /// Configures the specified startup sequence.
        /// </summary>
        /// <param name="app">Configure the app.</param>
        /// <param name="runServer">
        /// When called, begins running requests and returns a Task indicating when
        /// requests will no longer be run.
        /// </param>
        /// <returns>
        /// Task which indicates when the server is finished running and all resources are released
        /// </returns>
        Task Configure(IAppBuilder app, Func<Task> runServer);
    }
}

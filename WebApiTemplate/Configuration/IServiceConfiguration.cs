using System;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.Configuration
{
    /// <summary>
    /// Configures the service
    /// </summary>
    public interface IServiceConfiguration : IDelegatingServer
    {
        /// <summary>
        /// Configures the specified start options.
        /// </summary>
        /// <param name="startOptions">The start options.</param>
        void Configure(StartOptions startOptions);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Owin.Hosting;
using Owin;
using webapitmpl.Utility;

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
        /// <returns>
        /// TODO
        /// </returns>
        Task Configure(IAppBuilder app, Func<IAppBuilder, Task> runServer);
    }
}

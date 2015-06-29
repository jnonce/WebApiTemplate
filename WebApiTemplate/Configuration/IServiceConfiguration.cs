using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Owin.Hosting;
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
        /// <param name="builder">Configure the services in the container.</param>
        /// <param name="getContainer">Gets the container</param>
        /// <returns>
        /// Array of objects identifying the <see cref="T:Utility.IAppConfiguration"/> to run from
        /// the container
        /// </returns>
        void Configure(ContainerBuilder builder, Func<IContainer> getContainer);
    }
}

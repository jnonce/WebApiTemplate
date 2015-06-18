using Microsoft.Owin.Hosting;

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
        /// <param name="startup">The startup sequence.</param>
        void Configure(App_Start.Startup startup);
    }
}

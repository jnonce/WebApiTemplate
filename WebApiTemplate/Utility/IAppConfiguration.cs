using System;
using Owin;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Startup class
    /// </summary>
    public interface IAppConfiguration
    {
        /// <summary>
        /// Configures the Owin pipeline
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        void Configuration(IAppBuilder appBuilder);
    }

    /// <summary>
    /// Basic configuration classes which invokes a delegate
    /// </summary>
    public class AppConfiguration : IAppConfiguration
    {
        private Action<IAppBuilder> configurer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfiguration"/> class.
        /// </summary>
        /// <param name="configurer">The configurer.</param>
        public AppConfiguration(Action<IAppBuilder> configurer)
        {
            this.configurer = configurer;
        }

        /// <summary>
        /// Configures the Owin pipeline
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        public void Configuration(IAppBuilder appBuilder)
        {
            configurer(appBuilder);
        }
    }
}

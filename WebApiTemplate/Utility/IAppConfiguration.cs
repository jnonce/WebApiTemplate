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
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        object Id { get; }

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
        /// <param name="id">Id for the configuration object</param>
        /// <param name="configurer">The configurer.</param>
        public AppConfiguration(object id, Action<IAppBuilder> configurer)
        {
            this.Id = id;
            this.configurer = configurer;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public object Id
        {
            get; private set;
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

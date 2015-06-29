using System;
using System.Collections.Generic;
using Owin;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Startup class
    /// </summary>
    public interface IStartup
    {
        /// <summary>
        /// Configures the Owin pipeline
        /// </summary>
        void Configuration();
    }

    /// <summary>
    /// Basic configuration classes which invokes a delegate
    /// </summary>
    public class DelegateStartup : IStartup
    {
        private Action configurer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateStartup"/> class.
        /// </summary>
        /// <param name="configurer">The configurer.</param>
        public DelegateStartup(Action configurer)
        {
            this.configurer = configurer;
        }

        /// <summary>
        /// Configures the Owin pipeline
        /// </summary>
        public void Configuration()
        {
            configurer();
        }
    }
}
